#!/usr/bin/env python3
"""
永恒地牢 - 账户认证系统
SQLite数据库 + 邮箱验证码 + 密码加密 + 会话管理
"""

import sqlite3
import hashlib
import secrets
import time
import re
import os
import json
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.header import Header
from datetime import datetime, timedelta

# 数据库路径
DB_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'game_users.db')

# 配置文件路径
CONFIG_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'config.json')

# 验证码有效期（秒）
VERIFICATION_CODE_EXPIRY = 600  # 10分钟

# 会话有效期（秒）
SESSION_EXPIRY = 86400 * 7  # 7天


def load_config():
    """加载配置文件"""
    try:
        with open(CONFIG_PATH, 'r', encoding='utf-8') as f:
            return json.load(f)
    except:
        return {
            'smtp': {
                'enabled': False,
                'server': 'smtp.office365.com',
                'port': 587,
                'use_tls': True,
                'username': '',
                'password': '',
                'from_name': '永恒地牢',
                'from_email': ''
            }
        }


class Database:
    """数据库管理"""

    def __init__(self):
        self.conn = None
        self.init_db()

    def get_conn(self):
        if self.conn is None:
            self.conn = sqlite3.connect(DB_PATH, check_same_thread=False)
            self.conn.row_factory = sqlite3.Row
        return self.conn

    def init_db(self):
        """初始化数据库表"""
        conn = self.get_conn()
        cursor = conn.cursor()

        # 用户表
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                email TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                salt TEXT NOT NULL,
                email_verified INTEGER DEFAULT 0,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                last_login TIMESTAMP,
                game_data TEXT DEFAULT '{}',
                membership_type TEXT DEFAULT 'free',
                membership_expires_at TIMESTAMP,
                activation_key_id INTEGER,
                is_admin INTEGER DEFAULT 0,
                FOREIGN KEY (activation_key_id) REFERENCES activation_keys (id)
            )
        ''')

        # 激活密钥表
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS activation_keys (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                key_value TEXT UNIQUE NOT NULL,
                key_type TEXT DEFAULT 'normal',
                duration_days INTEGER DEFAULT 30,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                created_by TEXT DEFAULT 'system',
                used INTEGER DEFAULT 0,
                used_by INTEGER,
                used_at TIMESTAMP,
                batch_id TEXT,
                note TEXT
            )
        ''')

        # 验证码表
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS verification_codes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                email TEXT NOT NULL,
                code TEXT NOT NULL,
                purpose TEXT NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                expires_at TIMESTAMP NOT NULL,
                used INTEGER DEFAULT 0
            )
        ''')

        # 会话表
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS sessions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL,
                token TEXT UNIQUE NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                expires_at TIMESTAMP NOT NULL,
                last_active TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                ip_address TEXT,
                user_agent TEXT,
                FOREIGN KEY (user_id) REFERENCES users (id)
            )
        ''')

        # 登录日志表
        cursor.execute('''
            CREATE TABLE IF NOT EXISTS login_logs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER,
                username TEXT,
                success INTEGER,
                ip_address TEXT,
                user_agent TEXT,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                failure_reason TEXT
            )
        ''')

        conn.commit()
        print(f"[Database] 数据库初始化完成: {DB_PATH}")

    def execute(self, query, params=()):
        """执行SQL查询"""
        conn = self.get_conn()
        cursor = conn.cursor()
        cursor.execute(query, params)
        conn.commit()
        return cursor

    def fetch_one(self, query, params=()):
        """查询单条记录"""
        conn = self.get_conn()
        cursor = conn.cursor()
        cursor.execute(query, params)
        return cursor.fetchone()

    def fetch_all(self, query, params=()):
        """查询多条记录"""
        conn = self.get_conn()
        cursor = conn.cursor()
        cursor.execute(query, params)
        return cursor.fetchall()


class AuthSystem:
    """认证系统"""

    def __init__(self):
        self.db = Database()

    def hash_password(self, password, salt=None):
        """密码加密（PBKDF2 + salt）"""
        if salt is None:
            salt = secrets.token_hex(16)
        password_hash = hashlib.pbkdf2_hmac(
            'sha256',
            password.encode('utf-8'),
            salt.encode('utf-8'),
            100000  # 迭代次数
        ).hex()
        return password_hash, salt

    def verify_password(self, password, password_hash, salt):
        """验证密码"""
        new_hash, _ = self.hash_password(password, salt)
        return secrets.compare_digest(new_hash, password_hash)

    def generate_token(self):
        """生成会话令牌"""
        return secrets.token_hex(32)

    def generate_verification_code(self):
        """生成6位数字验证码"""
        return ''.join([str(secrets.randbelow(10)) for _ in range(6)])

    def validate_email(self, email):
        """验证邮箱格式"""
        pattern = r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$'
        return re.match(pattern, email) is not None

    def validate_username(self, username):
        """验证用户名格式（3-20位，字母数字下划线）"""
        pattern = r'^[a-zA-Z0-9_]{3,20}$'
        return re.match(pattern, username) is not None

    def validate_password_strength(self, password):
        """验证密码强度（至少6位）"""
        if len(password) < 6:
            return False, "密码至少需要6位"
        if len(password) > 100:
            return False, "密码不能超过100位"
        return True, "密码强度合格"

    def send_verification_email(self, email, code, purpose="register"):
        """
        发送验证码邮件
        支持SMTP真实发送（Outlook/QQ/163等），配置失败时回退到开发模式
        """
        purpose_text = {
            'register': '注册账户',
            'reset_password': '重置密码',
            'change_email': '修改邮箱'
        }.get(purpose, '验证')

        config = load_config()
        smtp_config = config.get('smtp', {})

        # 检查SMTP是否已配置
        smtp_enabled = smtp_config.get('enabled', False)
        smtp_username = smtp_config.get('username', '')
        smtp_password = smtp_config.get('password', '')

        is_configured = (
            smtp_enabled and
            smtp_username and
            smtp_password and
            '你的Outlook邮箱' not in smtp_username and
            '你的邮箱密码' not in smtp_password
        )

        if is_configured:
            # 使用SMTP真实发送邮件
            try:
                return self._send_smtp_email(email, code, purpose_text, smtp_config)
            except Exception as e:
                print(f"[SMTP] 邮件发送失败，回退到开发模式: {e}")
                # 发送失败，回退到开发模式
                self._send_dev_mode(email, code, purpose_text)
                return True
        else:
            # 开发模式：打印到控制台和日志文件
            self._send_dev_mode(email, code, purpose_text)
            return True

    def _send_smtp_email(self, to_email, code, purpose_text, smtp_config):
        """使用SMTP发送真实邮件"""
        smtp_server = smtp_config.get('server', 'smtp.office365.com')
        smtp_port = smtp_config.get('port', 587)
        use_tls = smtp_config.get('use_tls', True)
        username = smtp_config.get('username', '')
        password = smtp_config.get('password', '')
        from_name = smtp_config.get('from_name', '永恒地牢')
        from_email = smtp_config.get('from_email', username)

        # 创建邮件
        msg = MIMEMultipart('alternative')
        msg['From'] = f"{Header(from_name, 'utf-8').encode()} <{from_email}>"
        msg['To'] = to_email
        msg['Subject'] = Header(f'【永恒地牢】{purpose_text}验证码', 'utf-8')

        # HTML邮件内容
        html_content = f"""
        <html>
        <head>
            <meta charset="utf-8">
            <style>
                body {{
                    font-family: 'Microsoft YaHei', Arial, sans-serif;
                    background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
                    margin: 0;
                    padding: 20px;
                    color: #fff;
                }}
                .container {{
                    max-width: 500px;
                    margin: 0 auto;
                    background: rgba(255, 255, 255, 0.05);
                    border-radius: 20px;
                    padding: 30px;
                    border: 1px solid rgba(255, 255, 255, 0.1);
                }}
                .header {{
                    text-align: center;
                    margin-bottom: 20px;
                }}
                .header h1 {{
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    -webkit-background-clip: text;
                    -webkit-text-fill-color: transparent;
                    font-size: 28px;
                    margin: 0;
                }}
                .content {{
                    text-align: center;
                }}
                .purpose {{
                    font-size: 16px;
                    color: #aaa;
                    margin-bottom: 20px;
                }}
                .code-box {{
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    border-radius: 15px;
                    padding: 25px;
                    margin: 20px 0;
                    box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
                }}
                .code {{
                    font-size: 36px;
                    font-weight: bold;
                    letter-spacing: 8px;
                    color: #fff;
                    margin: 0;
                }}
                .info {{
                    font-size: 13px;
                    color: #888;
                    margin-top: 20px;
                    line-height: 1.6;
                }}
                .footer {{
                    text-align: center;
                    margin-top: 30px;
                    padding-top: 20px;
                    border-top: 1px solid rgba(255, 255, 255, 0.1);
                    font-size: 12px;
                    color: #666;
                }}
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h1>⚔️ 永恒地牢</h1>
                </div>
                <div class="content">
                    <p class="purpose">您正在进行【{purpose_text}】操作</p>
                    <div class="code-box">
                        <p class="code">{code}</p>
                    </div>
                    <p style="color: #aaa; font-size: 14px;">验证码有效期：10分钟</p>
                    <div class="info">
                        <p>⚠️ 请勿将验证码透露给他人</p>
                        <p>如非本人操作，请忽略此邮件</p>
                    </div>
                </div>
                <div class="footer">
                    <p>永恒地牢账户系统 | 此邮件由系统自动发送</p>
                </div>
            </div>
        </body>
        </html>
        """

        # 纯文本版本（备用）
        text_content = f"""
        【永恒地牢】{purpose_text}验证码

        您的验证码是：{code}

        验证码有效期：10分钟

        请勿将验证码透露给他人。
        如非本人操作，请忽略此邮件。

        永恒地牢账户系统
        """

        msg.attach(MIMEText(text_content, 'plain', 'utf-8'))
        msg.attach(MIMEText(html_content, 'html', 'utf-8'))

        # 连接SMTP服务器并发送
        print(f"[SMTP] 正在连接 {smtp_server}:{smtp_port}...")

        if use_tls:
            server = smtplib.SMTP(smtp_server, smtp_port, timeout=30)
            server.ehlo()
            server.starttls()
            server.ehlo()
        else:
            server = smtplib.SMTP_SSL(smtp_server, smtp_port, timeout=30)
            server.ehlo()

        server.login(username, password)
        server.sendmail(from_email, [to_email], msg.as_string())
        server.quit()

        print(f"[SMTP] 验证码邮件已发送至: {to_email}")

        # 同时记录到日志（不包含完整验证码，只记录前2位）
        log_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'verification_codes.log')
        with open(log_path, 'a', encoding='utf-8') as f:
            f.write(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] ")
            f.write(f"邮箱: {to_email}, 用途: {purpose_text}, 验证码: {code[:2]}*** (已通过SMTP发送)\n")

        return True

    def _send_dev_mode(self, email, code, purpose_text):
        """开发模式：打印验证码到控制台和日志文件"""
        print(f"\n{'='*50}")
        print(f"📧 【开发模式 - 验证码】")
        print(f"收件人: {email}")
        print(f"用途: {purpose_text}")
        print(f"验证码: {code}")
        print(f"有效期: 10分钟")
        print(f"提示: 配置config.json中的SMTP信息后可发送真实邮件")
        print(f"{'='*50}\n")

        # 保存到日志文件
        log_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'verification_codes.log')
        with open(log_path, 'a', encoding='utf-8') as f:
            f.write(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] ")
            f.write(f"邮箱: {email}, 用途: {purpose_text}, 验证码: {code} (开发模式)\n")

    # ==================== 激活密钥管理 ====================

    def generate_key_string(self):
        """生成密钥字符串，格式：ED-XXXX-XXXX-XXXX-XXXX"""
        chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789'  # 去掉易混淆字符
        parts = []
        for _ in range(4):
            part = ''.join(secrets.choice(chars) for _ in range(4))
            parts.append(part)
        return 'ED-' + '-'.join(parts)

    def generate_activation_keys(self, count=1, key_type='normal', duration_days=30, created_by='system', note=''):
        """批量生成激活密钥"""
        if count < 1 or count > 1000:
            return {'success': False, 'message': '生成数量必须在1-1000之间'}

        valid_types = ['trial', 'normal', 'premium', 'lifetime']
        if key_type not in valid_types:
            return {'success': False, 'message': f'密钥类型必须是: {", ".join(valid_types)}'}

        # lifetime类型duration_days设为0（永久）
        if key_type == 'lifetime':
            duration_days = 0

        batch_id = secrets.token_hex(8)
        keys = []

        for _ in range(count):
            # 确保密钥唯一
            while True:
                key_value = self.generate_key_string()
                existing = self.db.fetch_one("SELECT id FROM activation_keys WHERE key_value = ?", (key_value,))
                if not existing:
                    break

            self.db.execute(
                """INSERT INTO activation_keys
                   (key_value, key_type, duration_days, created_by, batch_id, note)
                   VALUES (?, ?, ?, ?, ?, ?)""",
                (key_value, key_type, duration_days, created_by, batch_id, note)
            )
            keys.append(key_value)

        print(f"[Auth] 生成了 {count} 个激活密钥 (批次: {batch_id}, 类型: {key_type})")

        return {
            'success': True,
            'message': f'成功生成 {count} 个激活密钥',
            'count': count,
            'batch_id': batch_id,
            'key_type': key_type,
            'duration_days': duration_days,
            'keys': keys
        }

    def validate_activation_key(self, key_value):
        """验证激活密钥是否有效"""
        if not key_value:
            return {'valid': False, 'message': '请输入激活密钥'}

        # 统一格式（去掉空格，转大写）
        key_value = key_value.strip().upper().replace(' ', '')

        key_record = self.db.fetch_one(
            "SELECT * FROM activation_keys WHERE key_value = ?",
            (key_value,)
        )

        if not key_record:
            return {'valid': False, 'message': '激活密钥不存在'}

        if key_record['used']:
            return {'valid': False, 'message': '该激活密钥已被使用'}

        return {
            'valid': True,
            'message': '激活密钥有效',
            'key_id': key_record['id'],
            'key_type': key_record['key_type'],
            'duration_days': key_record['duration_days']
        }

    def use_activation_key(self, key_value, user_id):
        """使用激活密钥"""
        validation = self.validate_activation_key(key_value)
        if not validation['valid']:
            return {'success': False, 'message': validation['message']}

        key_id = validation['key_id']
        key_type = validation['key_type']
        duration_days = validation['duration_days']

        # 计算过期时间
        if duration_days > 0:
            expires_at = (datetime.now() + timedelta(days=duration_days)).strftime('%Y-%m-%d %H:%M:%S')
        else:
            expires_at = None  # 永久

        # 标记密钥已使用
        self.db.execute(
            "UPDATE activation_keys SET used = 1, used_by = ?, used_at = CURRENT_TIMESTAMP WHERE id = ?",
            (user_id, key_id)
        )

        # 更新用户会员信息
        if expires_at:
            self.db.execute(
                "UPDATE users SET membership_type = ?, membership_expires_at = ?, activation_key_id = ? WHERE id = ?",
                (key_type, expires_at, key_id, user_id)
            )
        else:
            self.db.execute(
                "UPDATE users SET membership_type = ?, membership_expires_at = NULL, activation_key_id = ? WHERE id = ?",
                (key_type, key_id, user_id)
            )

        print(f"[Auth] 用户 {user_id} 使用了激活密钥 {key_value} (类型: {key_type})")

        return {
            'success': True,
            'message': '激活成功！',
            'key_type': key_type,
            'duration_days': duration_days,
            'expires_at': expires_at
        }

    def get_all_keys(self, include_used=True):
        """获取所有激活密钥"""
        if include_used:
            keys = self.db.fetch_all(
                "SELECT k.*, u.username as used_by_name FROM activation_keys k LEFT JOIN users u ON k.used_by = u.id ORDER BY k.id DESC"
            )
        else:
            keys = self.db.fetch_all(
                "SELECT k.*, u.username as used_by_name FROM activation_keys k LEFT JOIN users u ON k.used_by = u.id WHERE k.used = 0 ORDER BY k.id DESC"
            )
        return [dict(k) for k in keys]

    def get_key_stats(self):
        """获取密钥统计"""
        total = self.db.fetch_one("SELECT COUNT(*) as count FROM activation_keys")['count']
        used = self.db.fetch_one("SELECT COUNT(*) as count FROM activation_keys WHERE used = 1")['count']
        unused = total - used

        type_stats = {}
        for key_type in ['trial', 'normal', 'premium', 'lifetime']:
            count = self.db.fetch_one("SELECT COUNT(*) as count FROM activation_keys WHERE key_type = ?", (key_type,))['count']
            type_stats[key_type] = count

        return {
            'total': total,
            'used': used,
            'unused': unused,
            'by_type': type_stats
        }

    def check_membership(self, user_id):
        """检查用户会员状态"""
        user = self.db.fetch_one(
            "SELECT membership_type, membership_expires_at FROM users WHERE id = ?",
            (user_id,)
        )

        if not user:
            return {'active': False, 'message': '用户不存在'}

        membership_type = user['membership_type'] or 'free'

        # 永久会员
        if membership_type == 'lifetime':
            return {'active': True, 'type': 'lifetime', 'message': '永久会员', 'expires_at': None}

        # 检查是否过期
        if user['membership_expires_at']:
            expires_at = datetime.strptime(user['membership_expires_at'], '%Y-%m-%d %H:%M:%S')
            if datetime.now() > expires_at:
                return {'active': False, 'type': membership_type, 'message': '会员已过期', 'expires_at': user['membership_expires_at']}

            remaining_days = (expires_at - datetime.now()).days
            return {
                'active': True,
                'type': membership_type,
                'message': f'会员有效（剩余{remaining_days}天）',
                'expires_at': user['membership_expires_at'],
                'remaining_days': remaining_days
            }

        return {'active': False, 'type': 'free', 'message': '未激活', 'expires_at': None}

    def register_user(self, username, email, password, verification_code, activation_key=None):
        """注册新用户"""
        # 验证输入
        if not self.validate_username(username):
            return {'success': False, 'message': '用户名格式不正确（3-20位字母数字下划线）'}

        if not self.validate_email(email):
            return {'success': False, 'message': '邮箱格式不正确'}

        password_valid, password_msg = self.validate_password_strength(password)
        if not password_valid:
            return {'success': False, 'message': password_msg}

        # 检查用户名是否已存在
        existing_user = self.db.fetch_one("SELECT id FROM users WHERE username = ?", (username,))
        if existing_user:
            return {'success': False, 'message': '用户名已被注册'}

        # 检查邮箱是否已存在
        existing_email = self.db.fetch_one("SELECT id FROM users WHERE email = ?", (email,))
        if existing_email:
            return {'success': False, 'message': '邮箱已被注册'}

        # 验证验证码
        code_record = self.db.fetch_one(
            "SELECT * FROM verification_codes WHERE email = ? AND code = ? AND purpose = 'register' AND used = 0 ORDER BY id DESC LIMIT 1",
            (email, verification_code)
        )

        if not code_record:
            return {'success': False, 'message': '验证码不正确'}

        # 检查验证码是否过期
        expires_at = datetime.strptime(code_record['expires_at'], '%Y-%m-%d %H:%M:%S')
        if datetime.now() > expires_at:
            return {'success': False, 'message': '验证码已过期，请重新获取'}

        # 标记验证码已使用
        self.db.execute("UPDATE verification_codes SET used = 1 WHERE id = ?", (code_record['id'],))

        # 验证激活密钥（商业化授权）
        if not activation_key:
            return {'success': False, 'message': '请输入激活密钥'}

        key_validation = self.validate_activation_key(activation_key)
        if not key_validation['valid']:
            return {'success': False, 'message': key_validation['message']}

        # 加密密码
        password_hash, salt = self.hash_password(password)

        # 计算会员过期时间
        key_type = key_validation['key_type']
        duration_days = key_validation['duration_days']
        if duration_days > 0:
            membership_expires = (datetime.now() + timedelta(days=duration_days)).strftime('%Y-%m-%d %H:%M:%S')
        else:
            membership_expires = None

        # 创建用户
        if membership_expires:
            self.db.execute(
                """INSERT INTO users
                   (username, email, password_hash, salt, email_verified,
                    membership_type, membership_expires_at, activation_key_id)
                   VALUES (?, ?, ?, ?, 1, ?, ?, ?)""",
                (username, email, password_hash, salt, key_type, membership_expires, key_validation['key_id'])
            )
        else:
            self.db.execute(
                """INSERT INTO users
                   (username, email, password_hash, salt, email_verified,
                    membership_type, membership_expires_at, activation_key_id)
                   VALUES (?, ?, ?, ?, 1, ?, NULL, ?)""",
                (username, email, password_hash, salt, key_type, key_validation['key_id'])
            )

        # 标记激活密钥已使用
        user = self.db.fetch_one("SELECT * FROM users WHERE username = ?", (username,))
        self.db.execute(
            "UPDATE activation_keys SET used = 1, used_by = ?, used_at = CURRENT_TIMESTAMP WHERE id = ?",
            (user['id'], key_validation['key_id'])
        )

        print(f"[Auth] 新用户注册成功: {username} (ID: {user['id']}, 会员类型: {key_type})")

        return {
            'success': True,
            'message': '注册成功！',
            'user_id': user['id'],
            'username': username,
            'membership_type': key_type,
            'membership_expires_at': membership_expires
        }

    def login_user(self, username, password, ip_address=None, user_agent=None):
        """用户登录"""
        # 查找用户
        user = self.db.fetch_one("SELECT * FROM users WHERE username = ? OR email = ?", (username, username))

        if not user:
            # 记录失败日志
            self.db.execute(
                "INSERT INTO login_logs (username, success, ip_address, user_agent, failure_reason) VALUES (?, 0, ?, ?, '用户不存在')",
                (username, ip_address, user_agent)
            )
            return {'success': False, 'message': '用户名或密码错误'}

        # 验证密码
        if not self.verify_password(password, user['password_hash'], user['salt']):
            # 记录失败日志
            self.db.execute(
                "INSERT INTO login_logs (user_id, username, success, ip_address, user_agent, failure_reason) VALUES (?, ?, 0, ?, ?, '密码错误')",
                (user['id'], user['username'], ip_address, user_agent)
            )
            return {'success': False, 'message': '用户名或密码错误'}

        # 检查邮箱是否验证
        if not user['email_verified']:
            return {'success': False, 'message': '邮箱未验证，请先验证邮箱'}

        # 创建会话
        token = self.generate_token()
        expires_at = (datetime.now() + timedelta(seconds=SESSION_EXPIRY)).strftime('%Y-%m-%d %H:%M:%S')

        self.db.execute(
            "INSERT INTO sessions (user_id, token, expires_at, ip_address, user_agent) VALUES (?, ?, ?, ?, ?)",
            (user['id'], token, expires_at, ip_address, user_agent)
        )

        # 更新最后登录时间
        self.db.execute("UPDATE users SET last_login = CURRENT_TIMESTAMP WHERE id = ?", (user['id'],))

        # 记录成功日志
        self.db.execute(
            "INSERT INTO login_logs (user_id, username, success, ip_address, user_agent) VALUES (?, ?, 1, ?, ?)",
            (user['id'], user['username'], ip_address, user_agent)
        )

        print(f"[Auth] 用户登录成功: {user['username']} (ID: {user['id']})")

        return {
            'success': True,
            'message': '登录成功！',
            'token': token,
            'user_id': user['id'],
            'username': user['username'],
            'email': user['email'],
            'expires_at': expires_at
        }

    def logout_user(self, token):
        """用户登出"""
        self.db.execute("DELETE FROM sessions WHERE token = ?", (token,))
        return {'success': True, 'message': '已登出'}

    def verify_session(self, token):
        """验证会话"""
        if not token:
            return None

        session = self.db.fetch_one("SELECT * FROM sessions WHERE token = ?", (token,))
        if not session:
            return None

        # 检查是否过期
        expires_at = datetime.strptime(session['expires_at'], '%Y-%m-%d %H:%M:%S')
        if datetime.now() > expires_at:
            self.db.execute("DELETE FROM sessions WHERE id = ?", (session['id'],))
            return None

        # 更新最后活跃时间
        self.db.execute("UPDATE sessions SET last_active = CURRENT_TIMESTAMP WHERE id = ?", (session['id'],))

        # 获取用户信息
        user = self.db.fetch_one("SELECT id, username, email, email_verified, created_at FROM users WHERE id = ?", (session['user_id'],))
        return dict(user) if user else None

    def request_verification_code(self, email, purpose="register"):
        """请求验证码"""
        if not self.validate_email(email):
            return {'success': False, 'message': '邮箱格式不正确'}

        # 注册时检查邮箱是否已存在
        if purpose == 'register':
            existing = self.db.fetch_one("SELECT id FROM users WHERE email = ?", (email,))
            if existing:
                return {'success': False, 'message': '该邮箱已被注册'}

        # 生成验证码
        code = self.generate_verification_code()
        expires_at = (datetime.now() + timedelta(seconds=VERIFICATION_CODE_EXPIRY)).strftime('%Y-%m-%d %H:%M:%S')

        # 保存验证码
        self.db.execute(
            "INSERT INTO verification_codes (email, code, purpose, expires_at) VALUES (?, ?, ?, ?)",
            (email, code, purpose, expires_at)
        )

        # 发送验证码
        self.send_verification_email(email, code, purpose)

        return {
            'success': True,
            'message': '验证码已发送（查看控制台或verification_codes.log文件）',
            'code': code,  # 开发环境返回验证码，生产环境应移除
            'expires_in': VERIFICATION_CODE_EXPIRY
        }

    def get_user_game_data(self, user_id):
        """获取用户游戏数据"""
        user = self.db.fetch_one("SELECT game_data FROM users WHERE id = ?", (user_id,))
        if user and user['game_data']:
            try:
                return json.loads(user['game_data'])
            except:
                return {}
        return {}

    def save_user_game_data(self, user_id, game_data):
        """保存用户游戏数据"""
        data_json = json.dumps(game_data, ensure_ascii=False)
        self.db.execute("UPDATE users SET game_data = ? WHERE id = ?", (data_json, user_id))
        return {'success': True, 'message': '游戏数据已保存'}

    def get_user_stats(self):
        """获取用户统计"""
        total_users = self.db.fetch_one("SELECT COUNT(*) as count FROM users")['count']
        today_registrations = self.db.fetch_one(
            "SELECT COUNT(*) as count FROM users WHERE DATE(created_at) = DATE('now')"
        )['count']
        active_sessions = self.db.fetch_one("SELECT COUNT(*) as count FROM sessions WHERE expires_at > CURRENT_TIMESTAMP")['count']

        return {
            'total_users': total_users,
            'today_registrations': today_registrations,
            'active_sessions': active_sessions
        }


# 全局认证系统实例
auth_system = AuthSystem()


if __name__ == '__main__':
    # 测试
    print("=== 账户认证系统测试 ===")
    print(f"数据库路径: {DB_PATH}")
    print(f"用户统计: {auth_system.get_user_stats()}")
    print("\n测试发送验证码:")
    result = auth_system.request_verification_code("test@example.com", "register")
    print(result)
