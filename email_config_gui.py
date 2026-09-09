#!/usr/bin/env python3
"""
永恒地牢 - SMTP邮箱配置工具
图形界面配置，支持测试连接
"""

import tkinter as tk
from tkinter import ttk, messagebox
import json
import os
import smtplib
from email.mime.text import MIMEText
from email.header import Header
import threading

# 配置文件路径
CONFIG_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'config.json')


class SMTPConfigGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("⚔️ 永恒地牢 - 邮箱配置工具")
        self.root.geometry("500x550")
        self.root.resizable(False, False)

        # 设置窗口图标和背景
        self.root.configure(bg='#1a1a2e')

        # 加载现有配置
        self.config = self.load_config()

        self.create_widgets()

    def load_config(self):
        """加载配置文件"""
        try:
            with open(CONFIG_PATH, 'r', encoding='utf-8') as f:
                return json.load(f)
        except:
            return {
                'smtp': {
                    'enabled': True,
                    'server': 'smtp.office365.com',
                    'port': 587,
                    'use_tls': True,
                    'username': '',
                    'password': '',
                    'from_name': '永恒地牢',
                    'from_email': ''
                }
            }

    def save_config(self):
        """保存配置文件"""
        with open(CONFIG_PATH, 'w', encoding='utf-8') as f:
            json.dump(self.config, f, indent=2, ensure_ascii=False)

    def create_widgets(self):
        """创建界面组件"""
        # 标题
        title_frame = tk.Frame(self.root, bg='#1a1a2e')
        title_frame.pack(fill='x', pady=20)

        title_label = tk.Label(
            title_frame,
            text="⚔️ 永恒地牢",
            font=('Microsoft YaHei', 24, 'bold'),
            bg='#1a1a2e',
            fg='#667eea'
        )
        title_label.pack()

        subtitle_label = tk.Label(
            title_frame,
            text="邮箱验证码配置工具",
            font=('Microsoft YaHei', 12),
            bg='#1a1a2e',
            fg='#888'
        )
        subtitle_label.pack(pady=5)

        # 配置区域
        config_frame = tk.Frame(self.root, bg='#16213e', padx=20, pady=20)
        config_frame.pack(fill='x', padx=20)

        # SMTP服务器
        tk.Label(
            config_frame,
            text="SMTP服务器:",
            font=('Microsoft YaHei', 10),
            bg='#16213e',
            fg='#aaa'
        ).grid(row=0, column=0, sticky='w', pady=5)

        self.server_entry = tk.Entry(
            config_frame,
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#fff',
            insertbackground='#fff',
            relief='flat',
            width=30
        )
        self.server_entry.grid(row=0, column=1, pady=5, padx=10)
        self.server_entry.insert(0, self.config['smtp'].get('server', 'smtp.office365.com'))

        # 端口
        tk.Label(
            config_frame,
            text="端口:",
            font=('Microsoft YaHei', 10),
            bg='#16213e',
            fg='#aaa'
        ).grid(row=1, column=0, sticky='w', pady=5)

        self.port_entry = tk.Entry(
            config_frame,
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#fff',
            insertbackground='#fff',
            relief='flat',
            width=30
        )
        self.port_entry.grid(row=1, column=1, pady=5, padx=10)
        self.port_entry.insert(0, str(self.config['smtp'].get('port', 587)))

        # 邮箱地址
        tk.Label(
            config_frame,
            text="邮箱地址:",
            font=('Microsoft YaHei', 10),
            bg='#16213e',
            fg='#aaa'
        ).grid(row=2, column=0, sticky='w', pady=5)

        self.email_entry = tk.Entry(
            config_frame,
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#fff',
            insertbackground='#fff',
            relief='flat',
            width=30
        )
        self.email_entry.grid(row=2, column=1, pady=5, padx=10)
        self.email_entry.insert(0, self.config['smtp'].get('username', ''))

        # 邮箱密码
        tk.Label(
            config_frame,
            text="邮箱密码:",
            font=('Microsoft YaHei', 10),
            bg='#16213e',
            fg='#aaa'
        ).grid(row=3, column=0, sticky='w', pady=5)

        self.password_entry = tk.Entry(
            config_frame,
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#fff',
            insertbackground='#fff',
            relief='flat',
            width=30,
            show='*'
        )
        self.password_entry.grid(row=3, column=1, pady=5, padx=10)
        self.password_entry.insert(0, self.config['smtp'].get('password', ''))

        # 显示/隐藏密码
        self.show_password = tk.BooleanVar(value=False)
        self.show_password_check = tk.Checkbutton(
            config_frame,
            text="显示密码",
            variable=self.show_password,
            command=self.toggle_password,
            font=('Microsoft YaHei', 9),
            bg='#16213e',
            fg='#888',
            activebackground='#16213e',
            activeforeground='#667eea',
            selectcolor='#1a1a2e'
        )
        self.show_password_check.grid(row=4, column=1, sticky='w', padx=10)

        # 发件人名称
        tk.Label(
            config_frame,
            text="发件人名称:",
            font=('Microsoft YaHei', 10),
            bg='#16213e',
            fg='#aaa'
        ).grid(row=5, column=0, sticky='w', pady=5)

        self.from_name_entry = tk.Entry(
            config_frame,
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#fff',
            insertbackground='#fff',
            relief='flat',
            width=30
        )
        self.from_name_entry.grid(row=5, column=1, pady=5, padx=10)
        self.from_name_entry.insert(0, self.config['smtp'].get('from_name', '永恒地牢'))

        # 按钮区域
        button_frame = tk.Frame(self.root, bg='#1a1a2e')
        button_frame.pack(fill='x', pady=20)

        # 测试连接按钮
        self.test_button = tk.Button(
            button_frame,
            text="🔍 测试连接",
            font=('Microsoft YaHei', 11, 'bold'),
            bg='#f39c12',
            fg='#fff',
            activebackground='#e67e22',
            activeforeground='#fff',
            relief='flat',
            padx=20,
            pady=10,
            cursor='hand2',
            command=self.test_connection
        )
        self.test_button.pack(side='left', padx=10)

        # 保存配置按钮
        self.save_button = tk.Button(
            button_frame,
            text="💾 保存配置",
            font=('Microsoft YaHei', 11, 'bold'),
            bg='#2ecc71',
            fg='#fff',
            activebackground='#27ae60',
            activeforeground='#fff',
            relief='flat',
            padx=20,
            pady=10,
            cursor='hand2',
            command=self.save_configuration
        )
        self.save_button.pack(side='left', padx=10)

        # 发送测试邮件按钮
        self.send_test_button = tk.Button(
            button_frame,
            text="📧 发送测试邮件",
            font=('Microsoft YaHei', 11, 'bold'),
            bg='#3498db',
            fg='#fff',
            activebackground='#2980b9',
            activeforeground='#fff',
            relief='flat',
            padx=20,
            pady=10,
            cursor='hand2',
            command=self.send_test_email
        )
        self.send_test_button.pack(side='left', padx=10)

        # 状态显示
        self.status_label = tk.Label(
            self.root,
            text="💡 请填写邮箱信息，然后点击测试连接",
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#888',
            wraplength=450
        )
        self.status_label.pack(pady=10)

        # 帮助信息
        help_frame = tk.Frame(self.root, bg='#16213e', padx=15, pady=10)
        help_frame.pack(fill='x', padx=20, pady=10)

        help_text = """📌 Outlook邮箱配置说明：
• 服务器：smtp.office365.com（已预设）
• 端口：587（已预设）
• 密码：如果开启了两步验证，需要使用"应用专用密码"
• 应用专用密码获取：account.microsoft.com → 安全 → 高级安全选项 → 应用密码"""

        help_label = tk.Label(
            help_frame,
            text=help_text,
            font=('Microsoft YaHei', 9),
            bg='#16213e',
            fg='#666',
            justify='left',
            wraplength=440
        )
        help_label.pack(anchor='w')

    def toggle_password(self):
        """切换密码显示"""
        if self.show_password.get():
            self.password_entry.config(show='')
        else:
            self.password_entry.config(show='*')

    def update_status(self, message, color='#888'):
        """更新状态显示"""
        self.status_label.config(text=message, fg=color)
        self.root.update_idletasks()

    def get_config_from_form(self):
        """从表单获取配置"""
        return {
            'smtp': {
                'enabled': True,
                'server': self.server_entry.get().strip(),
                'port': int(self.port_entry.get().strip()),
                'use_tls': True,
                'username': self.email_entry.get().strip(),
                'password': self.password_entry.get(),
                'from_name': self.from_name_entry.get().strip(),
                'from_email': self.email_entry.get().strip()
            },
            'security': self.config.get('security', {
                'password_hash_iterations': 100000,
                'verification_code_expiry': 600,
                'session_expiry': 604800
            })
        }

    def test_connection(self):
        """测试SMTP连接"""
        email = self.email_entry.get().strip()
        password = self.password_entry.get()
        server = self.server_entry.get().strip()
        port = int(self.port_entry.get().strip())

        if not email or not password:
            messagebox.showwarning("提示", "请先填写邮箱地址和密码！")
            return

        self.update_status("⏳ 正在测试连接...", '#f39c12')
        self.test_button.config(state='disabled')

        # 在后台线程测试
        def test():
            try:
                if True:  # use_tls
                    smtp = smtplib.SMTP(server, port, timeout=15)
                    smtp.ehlo()
                    smtp.starttls()
                    smtp.ehlo()
                else:
                    smtp = smtplib.SMTP_SSL(server, port, timeout=15)
                    smtp.ehlo()

                smtp.login(email, password)
                smtp.quit()

                self.root.after(0, lambda: self.update_status("✅ 连接成功！邮箱配置正确，可以发送邮件", '#2ecc71'))
                self.root.after(0, lambda: messagebox.showinfo("成功", "SMTP连接成功！\n\n邮箱配置正确，可以发送验证码邮件了。"))

            except Exception as e:
                error_msg = str(e)
                self.root.after(0, lambda: self.update_status(f"❌ 连接失败: {error_msg}", '#e74c3c'))
                self.root.after(0, lambda: messagebox.showerror("失败", f"SMTP连接失败！\n\n错误信息: {error_msg}\n\n请检查：\n1. 邮箱地址是否正确\n2. 密码是否正确（开启两步验证需用应用专用密码）\n3. 网络是否正常"))

            finally:
                self.root.after(0, lambda: self.test_button.config(state='normal'))

        threading.Thread(target=test, daemon=True).start()

    def save_configuration(self):
        """保存配置"""
        email = self.email_entry.get().strip()
        password = self.password_entry.get()

        if not email or not password:
            messagebox.showwarning("提示", "请先填写邮箱地址和密码！")
            return

        self.config = self.get_config_from_form()
        self.save_config()

        self.update_status("✅ 配置已保存！重启服务器后生效", '#2ecc71')
        messagebox.showinfo("成功", "配置已保存！\n\n配置文件位置：\n" + CONFIG_PATH + "\n\n需要重启游戏服务器后才能生效。")

    def send_test_email(self):
        """发送测试邮件"""
        email = self.email_entry.get().strip()
        password = self.password_entry.get()
        server = self.server_entry.get().strip()
        port = int(self.port_entry.get().strip())
        from_name = self.from_name_entry.get().strip()

        if not email or not password:
            messagebox.showwarning("提示", "请先填写邮箱地址和密码！")
            return

        self.update_status("⏳ 正在发送测试邮件...", '#f39c12')
        self.send_test_button.config(state='disabled')

        def send():
            try:
                # 创建邮件
                msg = MIMEMultipart('alternative')
                msg['From'] = f"{Header(from_name, 'utf-8').encode()} <{email}>"
                msg['To'] = email
                msg['Subject'] = Header('【永恒地牢】测试邮件', 'utf-8')

                html_content = f"""
                <html>
                <head><meta charset="utf-8"></head>
                <body style="font-family: Microsoft YaHei, Arial; background: linear-gradient(135deg, #1a1a2e, #16213e); margin: 0; padding: 20px; color: #fff;">
                    <div style="max-width: 500px; margin: 0 auto; background: rgba(255,255,255,0.05); border-radius: 20px; padding: 30px; border: 1px solid rgba(255,255,255,0.1);">
                        <h1 style="text-align: center; background: linear-gradient(135deg, #667eea, #764ba2); -webkit-background-clip: text; -webkit-text-fill-color: transparent; font-size: 28px;">⚔️ 永恒地牢</h1>
                        <p style="text-align: center; color: #aaa; font-size: 16px;">邮箱配置测试成功！</p>
                        <div style="background: linear-gradient(135deg, #667eea, #764ba2); border-radius: 15px; padding: 25px; margin: 20px 0; text-align: center;">
                            <p style="font-size: 24px; font-weight: bold; color: #fff; margin: 0;">✅ 配置成功</p>
                        </div>
                        <p style="color: #aaa; font-size: 14px; text-align: center;">你的邮箱已经可以正常接收验证码了！</p>
                        <div style="text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid rgba(255,255,255,0.1); font-size: 12px; color: #666;">
                            永恒地牢账户系统 | 此邮件由系统自动发送
                        </div>
                    </div>
                </body>
                </html>
                """

                text_content = "【永恒地牢】测试邮件\n\n邮箱配置测试成功！\n你的邮箱已经可以正常接收验证码了。\n\n永恒地牢账户系统"

                msg.attach(MIMEText(text_content, 'plain', 'utf-8'))
                msg.attach(MIMEText(html_content, 'html', 'utf-8'))

                # 发送
                smtp = smtplib.SMTP(server, port, timeout=30)
                smtp.ehlo()
                smtp.starttls()
                smtp.ehlo()
                smtp.login(email, password)
                smtp.sendmail(email, [email], msg.as_string())
                smtp.quit()

                self.root.after(0, lambda: self.update_status("✅ 测试邮件已发送！请查收邮箱", '#2ecc71'))
                self.root.after(0, lambda: messagebox.showinfo("成功", f"测试邮件已发送！\n\n请查收邮箱：{email}\n\n如果收到邮件，说明邮箱配置完全正确。"))

            except Exception as e:
                error_msg = str(e)
                self.root.after(0, lambda: self.update_status(f"❌ 发送失败: {error_msg}", '#e74c3c'))
                self.root.after(0, lambda: messagebox.showerror("失败", f"发送测试邮件失败！\n\n错误信息: {error_msg}"))

            finally:
                self.root.after(0, lambda: self.send_test_button.config(state='normal'))

        threading.Thread(target=send, daemon=True).start()


def main():
    root = tk.Tk()
    app = SMTPConfigGUI(root)
    root.mainloop()


if __name__ == '__main__':
    main()
