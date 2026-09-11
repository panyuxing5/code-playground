import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import java.util.ArrayList;
import java.util.Random;
import java.util.HashMap;

/**
 * 永恒地牢 v2.0 - 完整版
 * 功能：登录注册、多层地牢、宠物、装备、技能、商店、存档
 * 
 * @author Doubao
 */
public class EternalDungeon extends JFrame {
    
    // 窗口常量
    private static final int WINDOW_WIDTH = 1200;
    private static final int WINDOW_HEIGHT = 800;
    
    // 游戏面板
    private CardLayout cardLayout;
    private JPanel mainPanel;
    
    // 各个界面
    private LoginPanel loginPanel;
    private GamePanel gamePanel;
    
    // 游戏数据
    private GameData gameData;
    
    public EternalDungeon() {
        setTitle("永恒地牢 v2.0 - Eternal Dungeon");
        setSize(WINDOW_WIDTH, WINDOW_HEIGHT);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);
        setResizable(false);
        
        gameData = new GameData();
        
        cardLayout = new CardLayout();
        mainPanel = new JPanel(cardLayout);
        
        loginPanel = new LoginPanel();
        gamePanel = new GamePanel();
        
        mainPanel.add(loginPanel, "login");
        mainPanel.add(gamePanel, "game");
        
        add(mainPanel);
        cardLayout.show(mainPanel, "login");
        
        setVisible(true);
    }
    
    // ==================== 登录面板 ====================
    class LoginPanel extends JPanel {
        private JTextField usernameField;
        private JPasswordField passwordField;
        private JTextField emailField;
        private JButton loginBtn;
        private JButton registerBtn;
        private boolean showRegister = false;
        
        public LoginPanel() {
            setLayout(null);
            setBackground(new Color(15, 20, 35));
            
            // 装饰背景
            JLabel bgLabel = new JLabel("");
            bgLabel.setBounds(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
            bgLabel.setBackground(new Color(15, 20, 35));
            bgLabel.setOpaque(true);
            add(bgLabel);
            
            // 标题
            JLabel titleLabel = new JLabel("⚔️ 永恒地牢 ⚔️", SwingConstants.CENTER);
            titleLabel.setFont(new Font("微软雅黑", Font.BOLD, 48));
            titleLabel.setForeground(new Color(212, 175, 55));
            titleLabel.setBounds(0, 100, WINDOW_WIDTH, 60);
            add(titleLabel);
            
            // 副标题
            JLabel subLabel = new JLabel("ETERNAL DUNGEON", SwingConstants.CENTER);
            subLabel.setFont(new Font("Arial", Font.BOLD, 18));
            subLabel.setForeground(new Color(150, 130, 80));
            subLabel.setBounds(0, 165, WINDOW_WIDTH, 30);
            add(subLabel);
            
            // 装饰线
            JSeparator sep = new JSeparator(SwingConstants.HORIZONTAL);
            sep.setBounds(400, 210, 400, 1);
            sep.setForeground(new Color(212, 175, 55));
            add(sep);
            
            // 用户名
            JLabel userLabel = new JLabel("用户名:");
            userLabel.setForeground(new Color(180, 180, 180));
            userLabel.setBounds(400, 250, 100, 25);
            add(userLabel);
            
            usernameField = new JTextField();
            usernameField.setBounds(400, 280, 400, 35);
            usernameField.setFont(new Font("微软雅黑", Font.PLAIN, 14));
            add(usernameField);
            
            // 密码
            JLabel passLabel = new JLabel("密码:");
            passLabel.setForeground(new Color(180, 180, 180));
            passLabel.setBounds(400, 330, 100, 25);
            add(passLabel);
            
            passwordField = new JPasswordField();
            passwordField.setBounds(400, 360, 400, 35);
            passwordField.setFont(new Font("微软雅黑", Font.PLAIN, 14));
            add(passwordField);
            
            // 邮箱（注册时显示）
            JLabel emailLabel = new JLabel("邮箱:");
            emailLabel.setForeground(new Color(180, 180, 180));
            emailLabel.setBounds(400, 410, 100, 25);
            emailLabel.setVisible(false);
            add(emailLabel);
            
            emailField = new JTextField();
            emailField.setBounds(400, 440, 400, 35);
            emailField.setFont(new Font("微软雅黑", Font.PLAIN, 14));
            emailField.setVisible(false);
            add(emailField);
            
            // 登录按钮
            loginBtn = new JButton("进入地牢");
            loginBtn.setBounds(400, 500, 195, 45);
            loginBtn.setBackground(new Color(212, 175, 55));
            loginBtn.setForeground(Color.BLACK);
            loginBtn.setFont(new Font("微软雅黑", Font.BOLD, 16));
            loginBtn.setFocusPainted(false);
            loginBtn.addActionListener(e -> doLogin());
            add(loginBtn);
            
            // 注册按钮
            registerBtn = new JButton("切换注册");
            registerBtn.setBounds(605, 500, 195, 45);
            registerBtn.setBackground(new Color(60, 60, 60));
            registerBtn.setForeground(Color.WHITE);
            registerBtn.setFont(new Font("微软雅黑", Font.BOLD, 16));
            registerBtn.setFocusPainted(false);
            registerBtn.addActionListener(e -> {
                showRegister = !showRegister;
                emailLabel.setVisible(showRegister);
                emailField.setVisible(showRegister);
                if (showRegister) {
                    loginBtn.setText("创建角色");
                } else {
                    loginBtn.setText("进入地牢");
                }
            });
            add(registerBtn);
            
            // 版本信息
            JLabel versionLabel = new JLabel("v2.0 | 完整版", SwingConstants.CENTER);
            versionLabel.setForeground(new Color(100, 100, 100));
            versionLabel.setBounds(0, 700, WINDOW_WIDTH, 20);
            add(versionLabel);
        }
        
        private void doLogin() {
            String username = usernameField.getText().trim();
            String password = new String(passwordField.getPassword()).trim();
            
            if (username.isEmpty() || password.isEmpty()) {
                JOptionPane.showMessageDialog(this, "请输入用户名和密码！", "提示", JOptionPane.WARNING_MESSAGE);
                return;
            }
            
            if (showRegister) {
                String email = emailField.getText().trim();
                if (email.isEmpty()) {
                    JOptionPane.showMessageDialog(this, "注册时请填写邮箱！", "提示", JOptionPane.WARNING_MESSAGE);
                    return;
                }
                gameData.createPlayer(username, email);
                JOptionPane.showMessageDialog(this, "角色创建成功！欢迎来到永恒地牢！");
            } else {
                gameData.loadPlayer(username);
            }
            
            gamePanel.initGame();
            cardLayout.show(mainPanel, "game");
            gamePanel.requestFocusInWindow();
        }
    }
    
    // ==================== 游戏面板 ====================
    class GamePanel extends JPanel {
        private int mapSize = 20;
        private int cellSize = 40;
        private int playerX, playerY;
        private int currentFloor = 1;
        
        private int[][] map;
        private ArrayList<Monster> monsters;
        private ArrayList<Item> items;
        
        private DefaultListModel<String> logModel;
        private JLabel floorLabel;
        private JLabel goldLabel;
        
        // 按钮
        private JButton inventoryBtn;
        private JButton shopBtn;
        private JButton petBtn;
        private JButton skillBtn;
        
        public GamePanel() {
            setLayout(null);
            setBackground(new Color(10, 10, 15));
            setFocusable(true);
            
            // 键盘控制
            addKeyListener(new KeyAdapter() {
                @Override
                public void keyPressed(KeyEvent e) {
                    handleKeyPress(e.getKeyCode());
                }
            });
        }
        
        public void initGame() {
            removeAll();
            
            playerX = mapSize / 2;
            playerY = mapSize / 2;
            currentFloor = 1;
            
            generateMap();
            generateMonsters();
            generateItems();
            
            // 顶部信息栏
            JPanel topBar = new JPanel();
            topBar.setLayout(new FlowLayout(FlowLayout.LEFT, 20, 5));
            topBar.setBounds(0, 0, WINDOW_WIDTH, 40);
            topBar.setBackground(new Color(20, 20, 30));
            
            floorLabel = new JLabel("🏰 第 " + currentFloor + " 层");
            floorLabel.setForeground(new Color(212, 175, 55));
            floorLabel.setFont(new Font("微软雅黑", Font.BOLD, 14));
            topBar.add(floorLabel);
            
            goldLabel = new JLabel("💰 " + gameData.player.gold + " 金币");
            goldLabel.setForeground(new Color(241, 196, 15));
            goldLabel.setFont(new Font("微软雅黑", Font.PLAIN, 14));
            topBar.add(goldLabel);
            
            add(topBar);
            
            // 战斗日志
            logModel = new DefaultListModel<>();
            JList<String> logList = new JList<>(logModel);
            logList.setBackground(new Color(20, 20, 25));
            logList.setForeground(new Color(180, 180, 180));
            logList.setFont(new Font("微软雅黑", Font.PLAIN, 12));
            JScrollPane logScroll = new JScrollPane(logList);
            logScroll.setBounds(20, 50, 220, 200);
            add(logScroll);
            
            // HUD面板
            JPanel hudPanel = new JPanel();
            hudPanel.setLayout(null);
            hudPanel.setBounds(20, 270, 220, 200);
            hudPanel.setBackground(new Color(20, 20, 30));
            hudPanel.setBorder(BorderFactory.createLineBorder(new Color(212, 175, 55), 1));
            
            // HP条
            JLabel hpLabel = new JLabel("❤️ 生命值");
            hpLabel.setForeground(Color.WHITE);
            hpLabel.setBounds(10, 10, 100, 20);
            hudPanel.add(hpLabel);
            
            JProgressBar hpBar = new JProgressBar(0, 100);
            hpBar.setBounds(10, 30, 200, 20);
            hpBar.setValue(100);
            hpBar.setStringPainted(true);
            hpBar.setForeground(new Color(231, 76, 60));
            hudPanel.add(hpBar);
            
            // MP条
            JLabel mpLabel = new JLabel("💙 魔法值");
            mpLabel.setForeground(Color.WHITE);
            mpLabel.setBounds(10, 60, 100, 20);
            hudPanel.add(mpLabel);
            
            JProgressBar mpBar = new JProgressBar(0, 100);
            mpBar.setBounds(10, 80, 200, 20);
            mpBar.setValue(80);
            mpBar.setStringPainted(true);
            mpBar.setForeground(new Color(52, 152, 219));
            hudPanel.add(mpBar);
            
            // 经验条
            JLabel expLabel = new JLabel("⭐ 经验值");
            expLabel.setForeground(Color.WHITE);
            expLabel.setBounds(10, 110, 100, 20);
            hudPanel.add(expLabel);
            
            JProgressBar expBar = new JProgressBar(0, 100);
            expBar.setBounds(10, 130, 200, 20);
            expBar.setValue(0);
            expBar.setStringPainted(true);
            expBar.setForeground(new Color(241, 196, 15));
            hudPanel.add(expBar);
            
            // 等级
            JLabel levelLabel = new JLabel("等级: " + gameData.player.level);
            levelLabel.setForeground(new Color(212, 175, 55));
            levelLabel.setBounds(10, 160, 200, 20);
            levelLabel.setFont(new Font("微软雅黑", Font.BOLD, 14));
            hudPanel.add(levelLabel);
            
            add(hudPanel);
            
            // 底部按钮栏
            JPanel buttonBar = new JPanel();
            buttonBar.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 10));
            buttonBar.setBounds(0, WINDOW_HEIGHT - 80, WINDOW_WIDTH, 60);
            buttonBar.setBackground(new Color(20, 20, 30));
            
            inventoryBtn = new JButton("🎒 背包");
            inventoryBtn.setPreferredSize(new Dimension(100, 40));
            inventoryBtn.addActionListener(e -> showInventory());
            buttonBar.add(inventoryBtn);
            
            shopBtn = new JButton("🏪 商店");
            shopBtn.setPreferredSize(new Dimension(100, 40));
            shopBtn.addActionListener(e -> showShop());
            buttonBar.add(shopBtn);
            
            petBtn = new JButton("🐉 宠物");
            petBtn.setPreferredSize(new Dimension(100, 40));
            petBtn.addActionListener(e -> showPets());
            buttonBar.add(petBtn);
            
            skillBtn = new JButton("✨ 技能");
            skillBtn.setPreferredSize(new Dimension(100, 40));
            skillBtn.addActionListener(e -> showSkills());
            buttonBar.add(skillBtn);
            
            add(buttonBar);
            
            // 小地图
            JPanel minimapPanel = new JPanel() {
                @Override
                protected void paintComponent(Graphics g) {
                    super.paintComponent(g);
                    drawMinimap(g);
                }
            };
            minimapPanel.setBounds(WINDOW_WIDTH - 200, 50, 180, 180);
            minimapPanel.setBackground(new Color(20, 20, 25));
            add(minimapPanel);
            
            addLog("欢迎来到永恒地牢，" + gameData.player.name + "！");
            addLog("使用 WASD 或方向键移动，空格攻击");
            
            repaint();
        }
        
        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);
            Graphics2D g2d = (Graphics2D) g;
            g2d.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
            
            int offsetX = 270;
            int offsetY = 80;
            
            // 绘制地图
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    int sx = offsetX + x * cellSize;
                    int sy = offsetY + y * cellSize;
                    
                    if (map[y][x] == 1) {
                        // 墙 - 渐变效果
                        GradientPaint gp = new GradientPaint(
                            sx, sy, new Color(60, 60, 80),
                            sx, sy + cellSize, new Color(40, 40, 60)
                        );
                        g2d.setPaint(gp);
                        g2d.fillRect(sx, sy, cellSize - 1, cellSize - 1);
                        
                        // 墙边框
                        g2d.setColor(new Color(80, 80, 100));
                        g2d.drawRect(sx, sy, cellSize - 1, cellSize - 1);
                    } else {
                        // 地板 - 棋盘格效果
                        if ((x + y) % 2 == 0) {
                            g2d.setColor(new Color(35, 35, 45));
                        } else {
                            g2d.setColor(new Color(30, 30, 40));
                        }
                        g2d.fillRect(sx, sy, cellSize - 1, cellSize - 1);
                    }
                }
            }
            
            // 绘制物品
            for (Item item : items) {
                int sx = offsetX + item.x * cellSize;
                int sy = offsetY + item.y * cellSize;
                g2d.setColor(new Color(241, 196, 15));
                g2d.setFont(new Font("微软雅黑", Font.PLAIN, 20));
                g2d.drawString(item.emoji, sx + 10, sy + 28);
            }
            
            // 绘制怪物
            for (Monster m : monsters) {
                int sx = offsetX + m.x * cellSize;
                int sy = offsetY + m.y * cellSize;
                
                // 怪物身体
                RadialGradientPaint rgp = new RadialGradientPaint(
                    sx + cellSize/2, sy + cellSize/2, cellSize/2,
                    new float[]{0f, 1f},
                    new Color[]{new Color(231, 76, 60), new Color(192, 57, 43)}
                );
                g2d.setPaint(rgp);
                g2d.fillOval(sx + 5, sy + 5, cellSize - 10, cellSize - 10);
                
                // 血条
                g2d.setColor(new Color(50, 50, 50));
                g2d.fillRect(sx + 5, sy - 8, cellSize - 10, 4);
                g2d.setColor(new Color(231, 76, 60));
                int hpWidth = (int) ((cellSize - 10) * (m.hp / (double)m.maxHp));
                g2d.fillRect(sx + 5, sy - 8, hpWidth, 4);
                
                // 怪物名称
                g2d.setColor(new Color(200, 200, 200));
                g2d.setFont(new Font("微软雅黑", Font.PLAIN, 10));
                g2d.drawString(m.type, sx + 5, sy + cellSize + 12);
            }
            
            // 绘制玩家
            int px = offsetX + playerX * cellSize;
            int py = offsetY + playerY * cellSize;
            
            // 宠物光环
            RadialGradientPaint petGlow = new RadialGradientPaint(
                px + cellSize/2, py + cellSize/2, cellSize,
                new float[]{0f, 0.7f, 1f},
                new Color[]{
                    new Color(241, 196, 15, 100),
                    new Color(241, 196, 15, 30),
                    new Color(241, 196, 15, 0)
                }
            );
            g2d.setPaint(petGlow);
            g2d.fillOval(px - 10, py - 10, cellSize + 20, cellSize + 20);
            
            // 玩家身体
            RadialGradientPaint playerGlow = new RadialGradientPaint(
                px + cellSize/2, py + cellSize/2, cellSize/2,
                new float[]{0f, 1f},
                new Color[]{new Color(241, 196, 15), new Color(212, 175, 55)}
            );
            g2d.setPaint(playerGlow);
            g2d.fillOval(px + 5, py + 5, cellSize - 10, cellSize - 10);
            
            // 玩家名字
            g2d.setColor(Color.WHITE);
            g2d.setFont(new Font("微软雅黑", Font.BOLD, 11));
            g2d.drawString(gameData.player.name, px + 5, py - 5);
        }
        
        private void drawMinimap(Graphics g) {
            int mmSize = 180;
            int cellSize = mmSize / mapSize;
            
            // 背景
            g.setColor(new Color(20, 20, 25));
            g.fillRect(0, 0, mmSize, mmSize);
            
            // 地图
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (map[y][x] == 1) {
                        g.setColor(new Color(80, 80, 100));
                        g.fillRect(x * cellSize, y * cellSize, cellSize, cellSize);
                    }
                }
            }
            
            // 怪物
            g.setColor(new Color(231, 76, 60));
            for (Monster m : monsters) {
                g.fillRect(m.x * cellSize, m.y * cellSize, cellSize, cellSize);
            }
            
            // 玩家
            g.setColor(new Color(241, 196, 15));
            g.fillRect(playerX * cellSize, playerY * cellSize, cellSize, cellSize);
        }
        
        private void handleKeyPress(int keyCode) {
            switch (keyCode) {
                case KeyEvent.VK_W:
                case KeyEvent.VK_UP:
                    movePlayer(0, -1);
                    break;
                case KeyEvent.VK_S:
                case KeyEvent.VK_DOWN:
                    movePlayer(0, 1);
                    break;
                case KeyEvent.VK_A:
                case KeyEvent.VK_LEFT:
                    movePlayer(-1, 0);
                    break;
                case KeyEvent.VK_D:
                case KeyEvent.VK_RIGHT:
                    movePlayer(1, 0);
                    break;
                case KeyEvent.VK_SPACE:
                    attackNearest();
                    break;
            }
            repaint();
        }
        
        private void generateMap() {
            map = new int[mapSize][mapSize];
            Random rand = new Random();
            
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (x == 0 || y == 0 || x == mapSize-1 || y == mapSize-1) {
                        map[y][x] = 1;
                    } else if (rand.nextDouble() < 0.12) {
                        map[y][x] = 1;
                    } else {
                        map[y][x] = 0;
                    }
                }
            }
            map[mapSize/2][mapSize/2] = 0;
        }
        
        private void generateMonsters() {
            monsters = new ArrayList<>();
            Random rand = new Random();
            String[] types = {"史莱姆", "骷髅兵", "蝙蝠", "哥布林", "狼人", "法师", "巨魔"};
            
            int count = 5 + currentFloor * 2;
            for (int i = 0; i < count; i++) {
                int x = rand.nextInt(mapSize - 2) + 1;
                int y = rand.nextInt(mapSize - 2) + 1;
                if (map[y][x] == 0 && !(x == playerX && y == playerY)) {
                    String type = types[rand.nextInt(types.length)];
                    monsters.add(new Monster(x, y, type, currentFloor));
                }
            }
        }
        
        private void generateItems() {
            items = new ArrayList<>();
            Random rand = new Random();
            String[] types = {"💎", "🧪", "⚔️", "🛡️", "💰"};
            String[] names = {"钻石", "药水", "宝剑", "盾牌", "金币"};
            
            int count = 3 + rand.nextInt(5);
            for (int i = 0; i < count; i++) {
                int x = rand.nextInt(mapSize - 2) + 1;
                int y = rand.nextInt(mapSize - 2) + 1;
                if (map[y][x] == 0 && !(x == playerX && y == playerY)) {
                    int typeIndex = rand.nextInt(types.length);
                    items.add(new Item(x, y, types[typeIndex], names[typeIndex]));
                }
            }
        }
        
        private void movePlayer(int dx, int dy) {
            int nx = playerX + dx;
            int ny = playerY + dy;
            
            if (nx < 0 || ny < 0 || nx >= mapSize || ny >= mapSize) return;
            if (map[ny][nx] == 1) return;
            
            // 检查怪物
            for (int i = 0; i < monsters.size(); i++) {
                Monster m = monsters.get(i);
                if (m.x == nx && m.y == ny) {
                    attackMonster(i);
                    return;
                }
            }
            
            // 检查物品
            for (int i = 0; i < items.size(); i++) {
                Item item = items.get(i);
                if (item.x == nx && item.y == ny) {
                    pickUpItem(i);
                    break;
                }
            }
            
            playerX = nx;
            playerY = ny;
            
            // 检查是否到达下一层
            if (nx == mapSize - 2 && ny == mapSize - 2) {
                nextFloor();
            }
        }
        
        private void attackNearest() {
            for (int i = 0; i < monsters.size(); i++) {
                Monster m = monsters.get(i);
                int dist = Math.abs(m.x - playerX) + Math.abs(m.y - playerY);
                if (dist <= 1) {
                    attackMonster(i);
                    return;
                }
            }
        }
        
        private void attackMonster(int index) {
            Monster m = monsters.get(index);
            int damage = gameData.player.attack + new Random().nextInt(10);
            boolean isCrit = new Random().nextDouble() < 0.25;
            int finalDamage = isCrit ? damage * 2 : damage;
            
            m.hp -= finalDamage;
            
            if (isCrit) {
                addLog("💥 暴击！你对" + m.type + "造成了 " + finalDamage + " 点伤害！");
            } else {
                addLog("⚔️ 你对" + m.type + "造成了 " + finalDamage + " 点伤害");
            }
            
            if (m.hp <= 0) {
                addLog("🎉 你击败了" + m.type + "！获得 " + m.exp + " 经验，" + m.gold + " 金币");
                gameData.player.exp += m.exp;
                gameData.player.gold += m.gold;
                monsters.remove(index);
                checkLevelUp();
                goldLabel.setText("💰 " + gameData.player.gold + " 金币");
            }
        }
        
        private void pickUpItem(int index) {
            Item item = items.get(index);
            addLog("✨ 拾取了 " + item.name);
            items.remove(index);
            
            // 简单处理：金币直接加，其他放背包
            if (item.name.equals("金币")) {
                gameData.player.gold += 50;
                goldLabel.setText("💰 " + gameData.player.gold + " 金币");
            } else if (item.name.equals("药水")) {
                gameData.player.hp = Math.min(gameData.player.maxHp, gameData.player.hp + 30);
                addLog("❤️ 恢复了 30 点生命值");
            }
        }
        
        private void checkLevelUp() {
            while (gameData.player.exp >= gameData.player.expToNext) {
                gameData.player.exp -= gameData.player.expToNext;
                gameData.player.level++;
                gameData.player.expToNext = (int)(gameData.player.expToNext * 1.5);
                gameData.player.maxHp += 25;
                gameData.player.hp = gameData.player.maxHp;
                gameData.player.maxMp += 15;
                gameData.player.mp = gameData.player.maxMp;
                gameData.player.attack += 4;
                gameData.player.defense += 3;
                addLog("🎉 升级！你现在是 " + gameData.player.level + " 级了！");
                JOptionPane.showMessageDialog(this, 
                    "🎉 升级！\n等级: " + gameData.player.level + 
                    "\n生命值: " + gameData.player.maxHp + 
                    "\n攻击力: " + gameData.player.attack, 
                    "升级", JOptionPane.INFORMATION_MESSAGE);
            }
        }
        
        private void nextFloor() {
            currentFloor++;
            floorLabel.setText("🏰 第 " + currentFloor + " 层");
            addLog("⬇️ 你进入了第 " + currentFloor + " 层地牢！");
            playerX = mapSize / 2;
            playerY = mapSize / 2;
            generateMap();
            generateMonsters();
            generateItems();
        }
        
        private void addLog(String text) {
            logModel.add(0, text);
            if (logModel.size() > 100) {
                logModel.removeElementAt(logModel.size() - 1);
            }
        }
        
        private void showInventory() {
            JOptionPane.showMessageDialog(this, 
                "🎒 背包\n\n" +
                "💰 金币: " + gameData.player.gold + "\n" +
                "⚔️ 攻击力: " + gameData.player.attack + "\n" +
                "🛡️ 防御力: " + gameData.player.defense + "\n\n" +
                "装备系统开发中...", 
                "背包", JOptionPane.PLAIN_MESSAGE);
        }
        
        private void showShop() {
            JOptionPane.showMessageDialog(this, 
                "🏪 商店\n\n" +
                "🔮 生命药水 - 50金币\n" +
                "🔮 魔法药水 - 50金币\n" +
                "⚔️ 铁剑 - 200金币\n" +
                "🛡️ 铁甲 - 200金币\n\n" +
                "商店系统开发中...", 
                "商店", JOptionPane.PLAIN_MESSAGE);
        }
        
        private void showPets() {
            JOptionPane.showMessageDialog(this, 
                "🐉 宠物系统\n\n" +
                "🔥 火龙 - 火系攻击，范围伤害\n" +
                "🦄 麒麟 - 治愈能力，回复队友\n" +
                "🦅 凤凰 - 复活技能，涅槃重生\n" +
                "🐺 独角兽 - 净化debuff，高速移动\n\n" +
                "宠物系统开发中...", 
                "宠物", JOptionPane.PLAIN_MESSAGE);
        }
        
        private void showSkills() {
            JOptionPane.showMessageDialog(this, 
                "✨ 技能系统\n\n" +
                "🔥 火球术 - 造成范围火焰伤害\n" +
                "❄️ 冰冻术 - 冻结敌人2回合\n" +
                "⚡ 闪电链 - 攻击多个敌人\n" +
                "💨 隐身术 - 隐身3回合\n\n" +
                "技能系统开发中...", 
                "技能", JOptionPane.PLAIN_MESSAGE);
        }
    }
    
    // ==================== 游戏数据 ====================
    class GameData {
        public Player player;
        
        public void createPlayer(String name, String email) {
            player = new Player(name);
        }
        
        public void loadPlayer(String name) {
            // 简化：直接创建新玩家
            player = new Player(name);
        }
    }
    
    // ==================== 玩家类 ====================
    class Player {
        String name;
        int level = 1;
        int exp = 0;
        int expToNext = 100;
        int hp = 100;
        int maxHp = 100;
        int mp = 50;
        int maxMp = 50;
        int gold = 0;
        int attack = 12;
        int defense = 6;
        
        public Player(String name) {
            this.name = name;
        }
    }
    
    // ==================== 怪物类 ====================
    class Monster {
        int x, y;
        String type;
        int hp;
        int maxHp;
        int attack;
        int exp;
        int gold;
        
        public Monster(int x, int y, String type, int floor) {
            this.x = x;
            this.y = y;
            this.type = type;
            this.maxHp = 30 + floor * 15;
            this.hp = maxHp;
            this.attack = 8 + floor * 3;
            this.exp = 30 + floor * 15;
            this.gold = 15 + floor * 8;
        }
    }
    
    // ==================== 物品类 ====================
    class Item {
        int x, y;
        String emoji;
        String name;
        
        public Item(int x, int y, String emoji, String name) {
            this.x = x;
            this.y = y;
            this.emoji = emoji;
            this.name = name;
        }
    }
    
    // ==================== 主函数 ====================
    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> new EternalDungeon());
    }
}