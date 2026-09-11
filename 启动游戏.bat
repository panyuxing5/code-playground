@echo off
chcp 65001 >nul
title 永恒地牢 v2.0
echo 正在启动永恒地牢...
cd /d "%~dp0src"
java EternalDungeon
pause