#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
永恒地牢游戏服务器 - 带后端API
支持静态文件服务 + 邮箱验证码API（开发模式直接显示验证码）
"""

import http.server
import socketserver
import json
import os
import random
import string
import urllib.parse
from datetime import datetime, timedelta

# 配置
PORT = 8081
STATIC_DIR = os.path.dirname(os.path.abspath(__file__))
VERIFICATION_CODES = {}  # email -> {code, expires_at}

class GameHandler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=STATIC_DIR, **kwargs)
    
    def do_POST(self):
        parsed = urllib.parse.urlparse(self.path)
        
        if parsed.path == '/api/auth/send-code':
            self.handle_send_code()
        elif parsed.path == '/api/auth/register':
            self.handle_register()
        elif parsed.path == '/api/auth/login':
            self.handle_login()
        else:
            self.send_error(404, 'Not Found')
    
    def handle_send_code(self):
        try:
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            data = json.loads(post_data.decode('utf-8'))
            email = data.get('email', '').strip()
            
            if not email:
                self.send_json_response({'success': False, 'message': '邮箱不能为空'})
                return
            
            # 生成6位验证码
            code = ''.join(random.choices(string.digits, k=6))
            expires_at = datetime.now() + timedelta(minutes=10)
            VERIFICATION_CODES[email] = {'code': code, 'expires_at': expires_at}
            
            print(f"[验证码] {email} -> {code} (10分钟有效)")
            
            # 开发模式：直接返回验证码，不真正发送邮件
            self.send_json_response({
                'success': True,
                'message': '验证码已发送',
                'code': code,  # 开发模式：前端会显示这个验证码
                'dev_mode': True
            })
            
        except Exception as e:
            print(f"[错误] 发送验证码失败: {e}")
            self.send_json_response({'success': False, 'message': '服务器错误'})
    
    def handle_register(self):
        try:
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            data = json.loads(post_data.decode('utf-8'))
            
            username = data.get('username', '').strip()
            email = data.get('email', '').strip()
            password = data.get('password', '')
            code = data.get('verification_code', '').strip()
            
            # 验证验证码
            if email in VERIFICATION_CODES:
                stored = VERIFICATION_CODES[email]
                if datetime.now() > stored['expires_at']:
                    del VERIFICATION_CODES[email]
                    self.send_json_response({'success': False, 'message': '验证码已过期'})
                    return
                if stored['code'] != code:
                    self.send_json_response({'success': False, 'message': '验证码错误'})
                    return
            else:
                self.send_json_response({'success': False, 'message': '请先获取验证码'})
                return
            
            # 注册成功（开发模式：不保存到数据库）
            print(f"[注册] 用户: {username}, 邮箱: {email}")
            
            self.send_json_response({
                'success': True,
                'message': '注册成功',
                'user': {
                    'id': random.randint(10000, 99999),
                    'username': username,
                    'email': email
                }
            })
            
        except Exception as e:
            print(f"[错误] 注册失败: {e}")
            self.send_json_response({'success': False, 'message': '服务器错误'})
    
    def handle_login(self):
        try:
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length)
            data = json.loads(post_data.decode('utf-8'))
            
            username = data.get('username', '').strip()
            password = data.get('password', '')
            
            # 开发模式：任意用户名密码都能登录
            print(f"[登录] 用户: {username}")
            
            self.send_json_response({
                'success': True,
                'message': '登录成功',
                'user': {
                    'id': random.randint(10000, 99999),
                    'username': username,
                    'email': f'{username}@dev.local'
                }
            })
            
        except Exception as e:
            print(f"[错误] 登录失败: {e}")
            self.send_json_response({'success': False, 'message': '服务器错误'})
    
    def send_json_response(self, data):
        response = json.dumps(data, ensure_ascii=False).encode('utf-8')
        self.send_response(200)
        self.send_header('Content-Type', 'application/json; charset=utf-8')
        self.send_header('Content-Length', len(response))
        self.send_header('Access-Control-Allow-Origin', '*')
        self.end_headers()
        self.wfile.write(response)
    
    def log_message(self, format, *args):
        # 简化日志输出
        if 'api' in str(args[0]):
            print(f"[{self.address_string()}] {format % args}")

def main():
    os.chdir(STATIC_DIR)
    
    with socketserver.ThreadingTCPServer(('0.0.0.0', PORT), GameHandler) as httpd:
        print(f"=" * 60)
        print(f"永恒地牢游戏服务器已启动")
        print(f"端口: {PORT}")
        print(f"静态目录: {STATIC_DIR}")
        print(f"开发模式: 验证码会直接显示在页面上")
        print(f"=" * 60)
        print(f"本机访问: http://127.0.0.1:{PORT}/")
        print(f"局域网访问: http://192.168.3.32:{PORT}/")
        print(f"=" * 60)
        
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\n服务器已停止")
            httpd.shutdown()

if __name__ == '__main__':
    main()
