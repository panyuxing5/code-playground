#!/usr/bin/env python3
"""
永恒地牢 - 激活密钥生成器
GUI工具，生成和管理激活密钥
"""

import tkinter as tk
from tkinter import ttk, messagebox, filedialog
import sqlite3
import os
import sys
import json
from datetime import datetime

# 数据库路径
DB_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'game_users.db')


class KeyGeneratorGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("⚔️ 永恒地牢 - 激活密钥生成器")
        self.root.geometry("900x650")
        self.root.resizable(True, True)
        self.root.configure(bg='#1a1a2e')

        # 初始化数据库
        self.init_database()

        self.create_widgets()
        self.refresh_stats()
        self.refresh_keys()

    def init_database(self):
        """初始化数据库表（如果不存在）"""
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()

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

        # 确保users表有会员字段
        try:
            cursor.execute("ALTER TABLE users ADD COLUMN membership_type TEXT DEFAULT 'free'")
        except:
            pass
        try:
            cursor.execute("ALTER TABLE users ADD COLUMN membership_expires_at TIMESTAMP")
        except:
            pass
        try:
            cursor.execute("ALTER TABLE users ADD COLUMN activation_key_id INTEGER")
        except:
            pass

        conn.commit()
        conn.close()

    def create_widgets(self):
        """创建界面组件"""
        # 主容器
        main_frame = tk.Frame(self.root, bg='#1a1a2e')
        main_frame.pack(fill='both', expand=True, padx=20, pady=20)

        # 标题
        title_frame = tk.Frame(main_frame, bg='#1a1a2e')
        title_frame.pack(fill='x', pady=(0, 20))

        tk.Label(
            title_frame,
            text="⚔️ 永恒地牢",
            font=('Microsoft YaHei', 24, 'bold'),
            bg='#1a1a2e',
            fg='#667eea'
        ).pack(side='left')

        tk.Label(
            title_frame,
            text="激活密钥生成器",
            font=('Microsoft YaHei', 14),
            bg='#1a1a2e',
            fg='#888'
        ).pack(side='left', padx=15, pady=8)

        # 统计卡片
        stats_frame = tk.Frame(main_frame, bg='#1a1a2e')
        stats_frame.pack(fill='x', pady=(0, 20))

        self.stats_labels = {}
        stats = [
            ('total', '总密钥数', '#667eea'),
            ('unused', '未使用', '#22c55e'),
            ('used', '已使用', '#f59e0b'),
            ('trial', '试用版', '#06b6d4'),
            ('normal', '普通版', '#8b5cf6'),
            ('premium', '高级版', '#ec4899'),
            ('lifetime', '永久版', '#ef4444'),
        ]

        for i, (key, label, color) in enumerate(stats):
            card = tk.Frame(stats_frame, bg='#16213e', padx=15, pady=12)
            card.grid(row=0, column=i, padx=5, sticky='nsew')
            stats_frame.grid_columnconfigure(i, weight=1)

            tk.Label(
                card,
                text=label,
                font=('Microsoft YaHei', 10),
                bg='#16213e',
                fg='#888'
            ).pack()

            value_label = tk.Label(
                card,
                text='0',
                font=('Microsoft YaHei', 20, 'bold'),
                bg='#16213e',
                fg=color
            )
            value_label.pack()
            self.stats_labels[key] = value_label

        # 生成区域
        gen_frame = tk.LabelFrame(
            main_frame,
            text=" 生成新密钥 ",
            font=('Microsoft YaHei', 12, 'bold'),
            bg='#16213e',
            fg='#667eea',
            padx=20,
            pady=15,
            bd=2,
            relief='groove'
        )
        gen_frame.pack(fill='x', pady=(0, 20))

        # 第一行：数量、类型、有效期
        row1 = tk.Frame(gen_frame, bg='#16213e')
        row1.pack(fill='x', pady=5)

        # 数量
        tk.Label(row1, text="生成数量:", font=('Microsoft YaHei', 11), bg='#16213e', fg='#aaa').pack(side='left', padx=(0, 10))
        self.count_var = tk.IntVar(value=10)
        count_spin = tk.Spinbox(
            row1,
            from_=1,
            to=1000,
            textvariable=self.count_var,
            width=8,
            font=('Microsoft YaHei', 11),
            bg='#1a1a2e',
            fg='white',
            insertbackground='white',
            relief='flat'
        )
        count_spin.pack(side='left', padx=(0, 20))

        # 类型
        tk.Label(row1, text="密钥类型:", font=('Microsoft YaHei', 11), bg='#16213e', fg='#aaa').pack(side='left', padx=(0, 10))
        self.type_var = tk.StringVar(value='normal')
        type_combo = ttk.Combobox(
            row1,
            textvariable=self.type_var,
            values=['trial', 'normal', 'premium', 'lifetime'],
            width=12,
            font=('Microsoft YaHei', 11),
            state='readonly'
        )
        type_combo.pack(side='left', padx=(0, 20))
        type_combo.bind('<<ComboboxSelected>>', self.on_type_change)

        # 有效期
        tk.Label(row1, text="有效期(天):", font=('Microsoft YaHei', 11), bg='#16213e', fg='#aaa').pack(side='left', padx=(0, 10))
        self.duration_var = tk.IntVar(value=30)
        duration_spin = tk.Spinbox(
            row1,
            from_=1,
            to=3650,
            textvariable=self.duration_var,
            width=8,
            font=('Microsoft YaHei', 11),
            bg='#1a1a2e',
            fg='white',
            insertbackground='white',
            relief='flat'
        )
        duration_spin.pack(side='left')

        # 第二行：备注、生成按钮
        row2 = tk.Frame(gen_frame, bg='#16213e')
        row2.pack(fill='x', pady=(15, 0))

        tk.Label(row2, text="备注:", font=('Microsoft YaHei', 11), bg='#16213e', fg='#aaa').pack(side='left', padx=(0, 10))
        self.note_var = tk.StringVar()
        note_entry = tk.Entry(
            row2,
            textvariable=self.note_var,
            width=40,
            font=('Microsoft YaHei', 11),
            bg='#1a1a2e',
            fg='white',
            insertbackground='white',
            relief='flat'
        )
        note_entry.pack(side='left', padx=(0, 20))

        # 生成按钮
        generate_btn = tk.Button(
            row2,
            text="🎫 生成密钥",
            font=('Microsoft YaHei', 12, 'bold'),
            bg='#667eea',
            fg='white',
            activebackground='#764ba2',
            activeforeground='white',
            relief='flat',
            padx=25,
            pady=8,
            cursor='hand2',
            command=self.generate_keys
        )
        generate_btn.pack(side='right')

        # 密钥列表区域
        list_frame = tk.LabelFrame(
            main_frame,
            text=" 密钥列表 ",
            font=('Microsoft YaHei', 12, 'bold'),
            bg='#16213e',
            fg='#667eea',
            padx=10,
            pady=10,
            bd=2,
            relief='groove'
        )
        list_frame.pack(fill='both', expand=True)

        # 工具栏
        toolbar = tk.Frame(list_frame, bg='#16213e')
        toolbar.pack(fill='x', pady=(0, 10))

        # 筛选
        tk.Label(toolbar, text="筛选:", font=('Microsoft YaHei', 10), bg='#16213e', fg='#888').pack(side='left', padx=(0, 5))
        self.filter_var = tk.StringVar(value='all')
        filter_combo = ttk.Combobox(
            toolbar,
            textvariable=self.filter_var,
            values=['all', 'unused', 'used', 'trial', 'normal', 'premium', 'lifetime'],
            width=12,
            font=('Microsoft YaHei', 10),
            state='readonly'
        )
        filter_combo.pack(side='left', padx=(0, 10))
        filter_combo.bind('<<ComboboxSelected>>', lambda e: self.refresh_keys())

        # 刷新按钮
        refresh_btn = tk.Button(
            toolbar,
            text="🔄 刷新",
            font=('Microsoft YaHei', 10),
            bg='#374151',
            fg='white',
            activebackground='#4b5563',
            activeforeground='white',
            relief='flat',
            padx=12,
            pady=4,
            cursor='hand2',
            command=self.refresh_all
        )
        refresh_btn.pack(side='left', padx=5)

        # 导出按钮
        export_btn = tk.Button(
            toolbar,
            text="📥 导出未使用密钥",
            font=('Microsoft YaHei', 10),
            bg='#059669',
            fg='white',
            activebackground='#047857',
            activeforeground='white',
            relief='flat',
            padx=12,
            pady=4,
            cursor='hand2',
            command=self.export_keys
        )
        export_btn.pack(side='left', padx=5)

        # 复制选中按钮
        copy_btn = tk.Button(
            toolbar,
            text="📋 复制选中",
            font=('Microsoft YaHei', 10),
            bg='#d97706',
            fg='white',
            activebackground='#b45309',
            activeforeground='white',
            relief='flat',
            padx=12,
            pady=4,
            cursor='hand2',
            command=self.copy_selected
        )
        copy_btn.pack(side='left', padx=5)

        # 表格
        columns = ('id', 'key_value', 'key_type', 'duration', 'status', 'used_by', 'created_at', 'note')
        self.tree = ttk.Treeview(list_frame, columns=columns, show='headings', height=12)

        # 列定义
        self.tree.heading('id', text='ID')
        self.tree.heading('key_value', text='密钥')
        self.tree.heading('key_type', text='类型')
        self.tree.heading('duration', text='有效期')
        self.tree.heading('status', text='状态')
        self.tree.heading('used_by', text='使用者')
        self.tree.heading('created_at', text='创建时间')
        self.tree.heading('note', text='备注')

        self.tree.column('id', width=50, anchor='center')
        self.tree.column('key_value', width=220, anchor='center')
        self.tree.column('key_type', width=80, anchor='center')
        self.tree.column('duration', width=70, anchor='center')
        self.tree.column('status', width=70, anchor='center')
        self.tree.column('used_by', width=100, anchor='center')
        self.tree.column('created_at', width=150, anchor='center')
        self.tree.column('note', width=120, anchor='center')

        # 滚动条
        scrollbar = ttk.Scrollbar(list_frame, orient='vertical', command=self.tree.yview)
        self.tree.configure(yscrollcommand=scrollbar.set)

        self.tree.pack(side='left', fill='both', expand=True)
        scrollbar.pack(side='right', fill='y')

        # 底部状态栏
        status_frame = tk.Frame(main_frame, bg='#1a1a2e')
        status_frame.pack(fill='x', pady=(10, 0))

        self.status_label = tk.Label(
            status_frame,
            text="就绪",
            font=('Microsoft YaHei', 10),
            bg='#1a1a2e',
            fg='#666'
        )
        self.status_label.pack(side='left')

    def on_type_change(self, event=None):
        """类型改变时调整有效期"""
        if self.type_var.get() == 'lifetime':
            self.duration_var.set(0)
        elif self.type_var.get() == 'trial':
            self.duration_var.set(7)
        elif self.type_var.get() == 'premium':
            self.duration_var.set(90)

    def generate_keys(self):
        """生成密钥"""
        count = self.count_var.get()
        key_type = self.type_var.get()
        duration = self.duration_var.get()
        note = self.note_var.get()

        if count < 1 or count > 1000:
            messagebox.showerror("错误", "生成数量必须在1-1000之间")
            return

        if key_type == 'lifetime':
            duration = 0

        # 生成密钥
        import secrets
        chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789'
        batch_id = secrets.token_hex(8)
        keys = []

        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()

        for _ in range(count):
            while True:
                parts = [''.join(secrets.choice(chars) for _ in range(4)) for _ in range(4)]
                key_value = 'ED-' + '-'.join(parts)
                cursor.execute("SELECT id FROM activation_keys WHERE key_value = ?", (key_value,))
                if not cursor.fetchone():
                    break

            cursor.execute(
                """INSERT INTO activation_keys
                   (key_value, key_type, duration_days, created_by, batch_id, note)
                   VALUES (?, ?, ?, 'keygen', ?, ?)""",
                (key_value, key_type, duration, batch_id, note)
            )
            keys.append(key_value)

        conn.commit()
        conn.close()

        self.status_label.config(text=f"成功生成 {count} 个密钥（批次：{batch_id}）")
        messagebox.showinfo("成功", f"成功生成 {count} 个密钥！\n\n批次ID：{batch_id}\n类型：{key_type}\n有效期：{duration}天")

        self.refresh_all()

    def refresh_stats(self):
        """刷新统计"""
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()

        stats = {}
        stats['total'] = cursor.execute("SELECT COUNT(*) FROM activation_keys").fetchone()[0]
        stats['used'] = cursor.execute("SELECT COUNT(*) FROM activation_keys WHERE used = 1").fetchone()[0]
        stats['unused'] = stats['total'] - stats['used']

        for key_type in ['trial', 'normal', 'premium', 'lifetime']:
            stats[key_type] = cursor.execute(
                "SELECT COUNT(*) FROM activation_keys WHERE key_type = ?",
                (key_type,)
            ).fetchone()[0]

        conn.close()

        for key, label in self.stats_labels.items():
            label.config(text=str(stats.get(key, 0)))

    def refresh_keys(self):
        """刷新密钥列表"""
        # 清空
        for item in self.tree.get_children():
            self.tree.delete(item)

        filter_type = self.filter_var.get()

        conn = sqlite3.connect(DB_PATH)
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        query = """
            SELECT k.*, u.username as used_by_name
            FROM activation_keys k
            LEFT JOIN users u ON k.used_by = u.id
        """
        params = []

        if filter_type == 'unused':
            query += " WHERE k.used = 0"
        elif filter_type == 'used':
            query += " WHERE k.used = 1"
        elif filter_type in ['trial', 'normal', 'premium', 'lifetime']:
            query += " WHERE k.key_type = ?"
            params.append(filter_type)

        query += " ORDER BY k.id DESC LIMIT 200"

        cursor.execute(query, params)
        rows = cursor.fetchall()

        type_names = {'trial': '试用', 'normal': '普通', 'premium': '高级', 'lifetime': '永久'}

        for row in rows:
            status = "已使用" if row['used'] else "未使用"
            duration = f"{row['duration_days']}天" if row['duration_days'] > 0 else "永久"
            used_by = row['used_by_name'] or '-'

            self.tree.insert('', 'end', values=(
                row['id'],
                row['key_value'],
                type_names.get(row['key_type'], row['key_type']),
                duration,
                status,
                used_by,
                row['created_at'],
                row['note'] or '-'
            ))

        conn.close()

    def refresh_all(self):
        """刷新所有"""
        self.refresh_stats()
        self.refresh_keys()

    def export_keys(self):
        """导出未使用密钥"""
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()
        cursor.execute("SELECT key_value, key_type, duration_days, note FROM activation_keys WHERE used = 0 ORDER BY id")
        rows = cursor.fetchall()
        conn.close()

        if not rows:
            messagebox.showinfo("提示", "没有未使用的密钥")
            return

        filepath = filedialog.asksaveasfilename(
            defaultextension=".txt",
            filetypes=[("文本文件", "*.txt"), ("CSV文件", "*.csv"), ("所有文件", "*.*")],
            initialfile=f"activation_keys_{datetime.now().strftime('%Y%m%d_%H%M%S')}.txt"
        )

        if not filepath:
            return

        with open(filepath, 'w', encoding='utf-8') as f:
            f.write("永恒地牢 - 激活密钥列表\n")
            f.write(f"导出时间：{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
            f.write(f"密钥数量：{len(rows)}\n")
            f.write("=" * 60 + "\n\n")

            for key_value, key_type, duration, note in rows:
                duration_str = f"{duration}天" if duration > 0 else "永久"
                f.write(f"{key_value}  [{key_type}/{duration_str}]\n")

        self.status_label.config(text=f"已导出 {len(rows)} 个密钥到 {filepath}")
        messagebox.showinfo("成功", f"已导出 {len(rows)} 个未使用密钥！\n\n文件：{filepath}")

    def copy_selected(self):
        """复制选中的密钥"""
        selected = self.tree.selection()
        if not selected:
            messagebox.showinfo("提示", "请先选择要复制的密钥")
            return

        keys = []
        for item in selected:
            values = self.tree.item(item, 'values')
            keys.append(values[1])  # key_value列

        text = '\n'.join(keys)
        self.root.clipboard_clear()
        self.root.clipboard_append(text)

        self.status_label.config(text=f"已复制 {len(keys)} 个密钥到剪贴板")
        messagebox.showinfo("成功", f"已复制 {len(keys)} 个密钥到剪贴板！")


def main():
    root = tk.Tk()

    # 设置主题样式
    style = ttk.Style()
    style.theme_use('clam')
    style.configure('Treeview',
                    background='#1a1a2e',
                    foreground='white',
                    fieldbackground='#1a1a2e',
                    rowheight=28,
                    font=('Microsoft YaHei', 10))
    style.configure('Treeview.Heading',
                    background='#374151',
                    foreground='white',
                    font=('Microsoft YaHei', 10, 'bold'))
    style.map('Treeview', background=[('selected', '#667eea')])

    app = KeyGeneratorGUI(root)
    root.mainloop()


if __name__ == '__main__':
    main()
