#!/usr/bin/env python3
"""
永恒地牢 - 带账户认证的HTTP服务器
扩展自安全服务器，添加注册/登录/验证码等API接口
"""

import http.server
import os
import urllib.parse
import json
import sys

# 导入认证系统
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from auth_system import auth_system

# 最严格安全响应头
SECURITY_HEADERS = {
    'Content-Security-Policy': (
        "default-src 'self'; "
        "script-src 'self'; "
        "style-src 'self'; "
        "img-src 'self' data:; "
        "font-src 'self'; "
        "connect-src 'self'; "
        "media-src 'self'; "
        "object-src 'none'; "
        "base-uri 'self'; "
        "frame-ancestors 'none'; "
        "form-action 'self'; "
        "frame-src 'none'; "
        "worker-src 'none'; "
        "manifest-src 'self'; "
        "fenced-frame-src 'none'"
    ),
    'X-Frame-Options': 'DENY',
    'X-Content-Type-Options': 'nosniff',
    'Referrer-Policy': 'no-referrer',
    'Permissions-Policy': (
        'camera=(), microphone=(), geolocation=(), '
        'interest-cohort=(), payment=(), usb=(), '
        'accelerometer=(), gyroscope=(), magnetometer=()'
    ),
    'X-XSS-Protection': '1; mode=block',
    'Cross-Origin-Opener-Policy': 'same-origin',
    'Cross-Origin-Resource-Policy': 'same-origin',
    'Cross-Origin-Embedder-Policy': 'require-corp',
    'Cache-Control': 'no-cache, no-store, must-revalidate',
    'Pragma': 'no-cache',
    'Expires': '0',
    'Strict-Transport-Security': 'max-age=31536000; includeSubDomains; preload',
    'X-Permitted-Cross-Domain-Policies': 'none',
    'X-DNS-Prefetch-Control': 'off',
}

# 文件扩展名白名单
ALLOWED_EXTENSIONS = {
    '.html', '.htm', '.js', '.css', '.json',
    '.png', '.jpg', '.jpeg', '.gif', '.svg', '.webp',
    '.mp3', '.wav', '.ogg', '.mp4', '.webm',
    '.woff', '.woff2', '.ttf', '.eot',
    '.ico', '.txt', '.map'
}

# 禁止访问的模式
BLOCKED_PATTERNS = [
    '..', '~', '.git', '.env', 'config',
    'backup', '.bak', '.swp', '.tmp',
    '__pycache__', '.DS_Store', '.py', '.db'
]


class AuthHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    """带认证的HTTP请求处理器"""

    server_version = "EternalDungeonServer"
    sys_version = ""

    def version_string(self):
        return "EternalDungeonServer"

    def send_response(self, code, message=None):
        super().send_response(code, message)
        for key, value in SECURITY_HEADERS.items():
            self.send_header(key, value)

    def send_json(self, data, status=200):
        """发送JSON响应"""
        self.send_response(status)
        self.send_header('Content-Type', 'application/json; charset=utf-8')
        self.end_headers()
        self.wfile.write(json.dumps(data, ensure_ascii=False).encode('utf-8'))

    def get_client_ip(self):
        """获取客户端IP"""
        return self.client_address[0] if self.client_address else 'unknown'

    def get_user_agent(self):
        """获取用户代理"""
        return self.headers.get('User-Agent', '')

    def get_auth_token(self):
        """从请求头获取认证令牌"""
        auth_header = self.headers.get('Authorization', '')
        if auth_header.startswith('Bearer '):
            return auth_header[7:]
        # 也可以从cookie获取
        cookie = self.headers.get('Cookie', '')
        if 'token=' in cookie:
            return cookie.split('token=')[1].split(';')[0]
        return None

    def do_GET(self):
        """处理GET请求"""
        # API路由
        if self.path.startswith('/api/'):
            self.handle_api_get()
            return

        # 静态文件
        self.handle_static_file()

    def do_POST(self):
        """处理POST请求"""
        # API路由
        if self.path.startswith('/api/'):
            self.handle_api_post()
            return

        self.send_json({'success': False, 'message': '不支持的请求'}, 405)

    def handle_api_get(self):
        """处理API GET请求"""
        path = self.path.split('?')[0]

        if path == '/api/auth/status':
            # 检查登录状态
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if user:
                self.send_json({'success': True, 'logged_in': True, 'user': user})
            else:
                self.send_json({'success': True, 'logged_in': False})

        elif path == '/api/auth/stats':
            # 用户统计（需要登录）
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            stats = auth_system.get_user_stats()
            self.send_json({'success': True, 'stats': stats})

        elif path == '/api/game/data':
            # 获取游戏数据
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            game_data = auth_system.get_user_game_data(user['id'])
            self.send_json({'success': True, 'game_data': game_data})

        elif path == '/api/keys/stats':
            # 密钥统计（需要管理员）
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            stats = auth_system.get_key_stats()
            self.send_json({'success': True, 'stats': stats})

        elif path == '/api/keys/list':
            # 密钥列表（需要管理员）
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            include_used = self.path.split('?')[1].get('include_used', 'true') == 'true' if '?' in self.path else True
            keys = auth_system.get_all_keys(include_used)
            self.send_json({'success': True, 'keys': keys})

        elif path == '/api/user/membership':
            # 用户会员状态
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            membership = auth_system.check_membership(user['id'])
            self.send_json({'success': True, 'membership': membership})

        else:
            self.send_json({'success': False, 'message': 'API不存在'}, 404)

    def handle_api_post(self):
        """处理API POST请求"""
        path = self.path.split('?')[0]

        # 读取请求体
        content_length = int(self.headers.get('Content-Length', 0))
        body = self.rfile.read(content_length) if content_length > 0 else b''

        try:
            data = json.loads(body.decode('utf-8')) if body else {}
        except:
            self.send_json({'success': False, 'message': '请求格式错误'}, 400)
            return

        if path == '/api/auth/register':
            # 注册（需要激活密钥）
            result = auth_system.register_user(
                username=data.get('username', ''),
                email=data.get('email', ''),
                password=data.get('password', ''),
                verification_code=data.get('verification_code', ''),
                activation_key=data.get('activation_key', '')
            )
            self.send_json(result, 200 if result['success'] else 400)

        elif path == '/api/keys/validate':
            # 验证激活密钥
            result = auth_system.validate_activation_key(data.get('key', ''))
            self.send_json(result)

        elif path == '/api/keys/generate':
            # 生成激活密钥（需要管理员）
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return

            result = auth_system.generate_activation_keys(
                count=data.get('count', 1),
                key_type=data.get('key_type', 'normal'),
                duration_days=data.get('duration_days', 30),
                created_by=user.get('username', 'unknown'),
                note=data.get('note', '')
            )
            self.send_json(result, 200 if result['success'] else 400)
            self.send_json(result, 200 if result['success'] else 400)

        elif path == '/api/auth/login':
            # 登录
            result = auth_system.login_user(
                username=data.get('username', ''),
                password=data.get('password', ''),
                ip_address=self.get_client_ip(),
                user_agent=self.get_user_agent()
            )
            self.send_json(result, 200 if result['success'] else 401)

        elif path == '/api/auth/logout':
            # 登出
            token = self.get_auth_token()
            result = auth_system.logout_user(token)
            self.send_json(result)

        elif path == '/api/auth/send-code':
            # 发送验证码
            result = auth_system.request_verification_code(
                email=data.get('email', ''),
                purpose=data.get('purpose', 'register')
            )
            self.send_json(result, 200 if result['success'] else 400)

        elif path == '/api/game/save':
            # 保存游戏数据
            token = self.get_auth_token()
            user = auth_system.verify_session(token)
            if not user:
                self.send_json({'success': False, 'message': '未登录'}, 401)
                return
            result = auth_system.save_user_game_data(user['id'], data.get('game_data', {}))
            self.send_json(result)

        else:
            self.send_json({'success': False, 'message': 'API不存在'}, 404)

    def handle_static_file(self):
        """处理静态文件请求"""
        # 解析路径
        parsed_path = urllib.parse.urlparse(self.path)
        file_path = parsed_path.path

        # 根路径默认index.html
        if file_path == '/':
            file_path = '/index.html'

        # 安全检查
        for pattern in BLOCKED_PATTERNS:
            if pattern in file_path:
                self.send_json({'success': False, 'message': '禁止访问'}, 403)
                return

        # 获取文件扩展名
        _, ext = os.path.splitext(file_path)
        ext = ext.lower()

        # 检查扩展名白名单
        if ext and ext not in ALLOWED_EXTENSIONS:
            self.send_json({'success': False, 'message': '不支持的文件类型'}, 403)
            return

        # 构建完整文件路径
        full_path = os.path.join(self.directory, file_path.lstrip('/'))

        # 检查文件是否存在
        if not os.path.isfile(full_path):
            self.send_json({'success': False, 'message': '文件不存在'}, 404)
            return

        # 发送文件
        try:
            with open(full_path, 'rb') as f:
                content = f.read()

            # 确定Content-Type
            content_types = {
                '.html': 'text/html; charset=utf-8',
                '.htm': 'text/html; charset=utf-8',
                '.js': 'application/javascript; charset=utf-8',
                '.css': 'text/css; charset=utf-8',
                '.json': 'application/json; charset=utf-8',
                '.png': 'image/png',
                '.jpg': 'image/jpeg',
                '.jpeg': 'image/jpeg',
                '.gif': 'image/gif',
                '.svg': 'image/svg+xml',
                '.webp': 'image/webp',
                '.mp3': 'audio/mpeg',
                '.wav': 'audio/wav',
                '.ogg': 'audio/ogg',
                '.mp4': 'video/mp4',
                '.webm': 'video/webm',
                '.woff': 'font/woff',
                '.woff2': 'font/woff2',
                '.ttf': 'font/ttf',
                '.eot': 'application/vnd.ms-fontobject',
                '.ico': 'image/x-icon',
                '.txt': 'text/plain; charset=utf-8',
            }

            content_type = content_types.get(ext, 'application/octet-stream')

            self.send_response(200)
            self.send_header('Content-Type', content_type)
            self.send_header('Content-Length', len(content))
            self.end_headers()
            self.wfile.write(content)

        except Exception as e:
            self.send_json({'success': False, 'message': f'读取文件错误: {str(e)}'}, 500)

    def log_message(self, format, *args):
        """简化日志"""
        print(f"[{self.log_date_time_string()}] {format % args}")


def run_server(port=8081, directory=None):
    """启动服务器"""
    if directory is None:
        directory = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'docs')

    # 设置工作目录
    os.chdir(directory)

    server_address = ('0.0.0.0', port)
    httpd = http.server.HTTPServer(server_address, AuthHTTPRequestHandler)

    print(f"\n{'='*60}")
    print(f"🎮 永恒地牢 - 账户认证服务器")
    print(f"{'='*60}")
    print(f"📂 静态文件目录: {directory}")
    print(f"🌐 本地地址: http://127.0.0.1:{port}/")
    print(f"📊 局域网地址: http://192.168.3.32:{port}/")
    print(f"🔐 API接口:")
    print(f"   POST /api/auth/register    - 注册")
    print(f"   POST /api/auth/login       - 登录")
    print(f"   POST /api/auth/logout      - 登出")
    print(f"   POST /api/auth/send-code   - 发送验证码")
    print(f"   GET  /api/auth/status       - 检查登录状态")
    print(f"   GET  /api/game/data         - 获取游戏数据")
    print(f"   POST /api/game/save         - 保存游戏数据")
    print(f"{'='*60}\n")

    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\n服务器已停止")
        httpd.server_close()


if __name__ == '__main__':
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8081
    directory = sys.argv[2] if len(sys.argv) > 2 else None
    run_server(port, directory)
