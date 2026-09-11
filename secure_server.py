#!/usr/bin/env python3
"""
安全加固的 HTTP 服务器 v3
最严格 CSP - 无通配符 / 无 scheme source / 无 unsafe-*
"""

import http.server
import os
import urllib.parse

# 最严格安全响应头
SECURITY_HEADERS = {
    'Content-Security-Policy': (
        "default-src 'self'; "
        "script-src 'self'; "
        "style-src 'self'; "
        "img-src 'self'; "
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
        ""
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
    '__pycache__', '.DS_Store'
]


class SecureHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    server_version = "SecureServer"
    sys_version = ""

    def version_string(self):
        return "SecureServer"

    def send_response(self, code, message=None):
        super().send_response(code, message)
        for key, value in SECURITY_HEADERS.items():
            self.send_header(key, value)

    def do_GET(self):
        parsed_path = urllib.parse.urlparse(self.path)
        clean_path = urllib.parse.unquote(parsed_path.path)

        # 路径遍历防护
        if '..' in clean_path or '~' in clean_path:
            self.send_error(403, "Forbidden")
            return

        # 敏感文件防护
        for pattern in BLOCKED_PATTERNS:
            if pattern in clean_path.lower():
                self.send_error(403, "Forbidden")
                return

        # 禁用目录列表
        if clean_path.endswith('/'):
            index_file = os.path.join(self.directory, clean_path.lstrip('/'), 'index.html')
            if os.path.exists(index_file):
                self.path = clean_path + 'index.html'
                if parsed_path.query:
                    self.path += '?' + parsed_path.query
            else:
                self.send_error(403, "Directory listing denied")
                return

        # 扩展名白名单
        _, ext = os.path.splitext(clean_path)
        if ext and ext.lower() not in ALLOWED_EXTENSIONS:
            self.send_error(403, "File type not allowed")
            return

        super().do_GET()

    def do_HEAD(self):
        self.do_GET()

    def log_message(self, format, *args):
        client_address = self.client_address[0]
        request_line = args[0] if args else ""
        status_code = args[1] if len(args) > 1 else ""
        print("[SecureServer] {} - {} - {}".format(client_address, request_line, status_code))

    def end_headers(self):
        super().end_headers()


def run_server(port=8081, directory="."):
    os.chdir(directory)
    server_address = ('0.0.0.0', port)
    httpd = http.server.HTTPServer(server_address, SecureHTTPRequestHandler)

    print("=" * 60)
    print("  安全加固 HTTP 服务器 v3 已启动")
    print("  端口: {}".format(port))
    print("  目录: {}".format(os.getcwd()))
    print("=" * 60)
    print("  CSP: 最严格模式（无通配符/无scheme/无unsafe）")
    print("=" * 60)

    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\n服务器已停止")
        httpd.server_close()


if __name__ == '__main__':
    import sys
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8081
    directory = sys.argv[2] if len(sys.argv) > 2 else "."
    run_server(port, directory)

