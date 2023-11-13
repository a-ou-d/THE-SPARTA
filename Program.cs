using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace THE_SPARTA
{
    // class로 Monster 생성
    public class Monster
    {
        public string MonsterName { get; set; }
        public int MonsterLevel { get; set; }
        public int MonsterAtk { get; set; }
        public int MonsterDef { get; set; }
        public int MonsterWis { get; set; }
        public int MonsterDex { get; set; }
        public int MonsterHp { get; set; }

        public Monster(string monsterName, string job, int monsterLevel, int monsterAtk, int monsterDef, int monsterWis, int monsterDex, int monsterHp)
        {
            MonsterName = monsterName;
            MonsterLevel = monsterLevel;
            MonsterAtk = monsterAtk;
            MonsterDef = monsterDef;
            MonsterWis = monsterWis;
            MonsterDex = monsterDex;
            MonsterHp = monsterHp;
        }
    }

    // class로 Player 생성
    public class Player
    {
        // 기본 능력치
        public string Name { get; set; }
        public string Job { get; set; }
        public int Level { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Wis { get; set; }
        public int Dex { get; set; }
        public int Hp { get; set; }
        public int Gold { get; set; }

        // 장비능력치
        public int EquipAtk { get; set; }
        public int EquipDef { get; set; }
        public int EquipWis { get; set; }
        public int EquipDex { get; set; }
        public int EquipHp { get; set; }

        // 돈
        public int CostGold { get; set; }
        public int TotalGold => Gold + CostGold;

        public List<Item> Store { get; set; } = new List<Item>();

        public void ItemBought(Item item)
        {
            if (!item.IsBought)
            {
                item.IsBought = true;
                CostGold -= item.ItemGold;
            }
        }

        public void ItemSold(Item item)
        {
            if (!item.IsSold)
            {
                item.IsSold = true;
                CostGold += item.ItemGold;
            }
        }

        public int TotalAtk => Atk + EquipAtk;
        public int TotalDef => Def + EquipDef;
        public int TotalWis => Wis + EquipWis;
        public int TotalDex => Dex + EquipDex;

        public List<Item> Inventory { get; set; } = new List<Item>();

        private Item[] items;

        public Player(string name, string job, int level, int atk, int def, int wis, int dex, int hp, int gold, Item[] items)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Wis = wis;
            Dex = dex;
            Hp = hp;
            Gold = gold;
            this.items = items;
        }

        public void TotalStats()
        {
            EquipAtk = 0;
            EquipDef = 0;
            EquipWis = 0;
            EquipDex = 0;

            foreach (Item item in Inventory)
            {
                if (item.IsEquipped)
                {
                    EquipAtk += item.ItemAtk;
                    EquipDef += item.ItemDef;
                    EquipWis += item.ItemWis;
                    EquipDex += item.ItemDex;
                }
            }
        }

        public void EquipItem(Item item)
        {
            item.IsEquipped = !item.IsEquipped;
            TotalStats();
        }

        public void DefaultEquip()
        {
            Dictionary<string, List<int>> defaultEquipment = new Dictionary<string, List<int>>
            {
                { "전사", new List<int> { 0, 1, 2, 3, 4, 5 } },
                { "마법사", new List<int> { 0, 1, 2, 3, 4, 6 } },
                { "궁수", new List<int> { 0, 1, 2, 3, 4, 7 } }
            };

            if (defaultEquipment.ContainsKey(Job))
            {
                foreach (int itemIndex in defaultEquipment[Job])
                {
                    Inventory.Add(items[itemIndex]);
                }
            }
        }
    }

    // class로 Item 생성
    public class Item
    {
        public string ItemName { get; set; }
        public int ItemAtk { get; set; }
        public int ItemDef { get; set; }
        public int ItemWis { get; set; }
        public int ItemDex { get; set; }
        public string ItemDescription { get; set; }
        public int ItemGold { get; set; }
        public int EquipAll { get; set; } = -1;
        public int EquipJob { get; set; } = -1;
        public bool IsEquipped { get; set; } = false;
        public bool IsBought { get; set; } = false;
        public bool IsSold { get; set; } = false;

        public Item(string itemName, int itemAtk, int itemDef, int itemWis, int itemDex, string itemDescription, int itemGold)
        {
            ItemName = itemName;
            ItemAtk = itemAtk;
            ItemDef = itemDef;
            ItemWis = itemWis;
            ItemDex = itemDex;
            ItemDescription = itemDescription;
            ItemGold = itemGold;
        }
    }

    internal class Program
    {
        private static Player player;
        private static Item item;
        private static Monster monster;

        // # 콘솔 설정
        static void ConsoleSetting()
        {
            Console.Title = "THE_SPARTA";
        }

        // #1. 타이틀 화면
        static void TitleDraw()
        {

                Console.WriteLine("\n\n\n\n");
                Console.WriteLine("                     ########           ##    ##           ####### ");
                Console.WriteLine("                        ##              ##    ##           ##      ");
                Console.WriteLine("                        ##              ########           ####### ");
                Console.WriteLine("                        ##              ##    ##           ##      ");
                Console.WriteLine("                        ##              ##    ##           ####### ");
                Console.WriteLine("\n\n");
                Console.WriteLine("   #######           ########           ########           ########             ###     ");
                Console.WriteLine("  ##                 ##      #          ##     ##             ##               ## ##    ");
                Console.WriteLine("   #######           ########           ########              ##              #######   ");
                Console.WriteLine("         ##          ##                 ##     ##             ##             ##     ##  ");
                Console.WriteLine("   #######           ##                 ##      ##            ##            ##       ## ");
                Console.WriteLine("\n\n\n");
        }

        // #2. 이름 입력받기
        static void Getname()
        {
            Console.WriteLine("\n\n\n\n\n\n\n");
            Console.WriteLine("이름을 알려주세요!");
            string playerName = Console.ReadLine();

            player = new Player(playerName, "", 0, 0, 0, 0, 0, 0, 0, items);
            
            Jobchoice(player);
            SelectJob();
        }

        // 직업-플레이어 연결용
        static void Jobchoice(Player player)
        {
            Job selectedJob = SelectJob();
        }

        // #3. 직업 입력받기 및 직업 능력치 설정
        static Job SelectJob()
        {
            Job selectedJob;

                do
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n\n\n\n");
                    Console.WriteLine(player.Name + "님, 반가워요! 직업은 무엇인가요?");
                    Console.WriteLine("[1] 전사");
                    Console.WriteLine("[2] 마법사");
                    Console.WriteLine("[3] 궁수");
                    string jobInput = Console.ReadLine();

                    if (int.TryParse(jobInput, out int jobNumber) && Enum.IsDefined(typeof(Job), jobNumber))
                    {
                        selectedJob = (Job)jobNumber;

                        switch (selectedJob)
                        {
                            case Job.Warrior:
                                player.Job = "전사";
                                player.Level = 1;
                                player.Atk = 10;
                                player.Def = 10;
                                player.Wis = 3;
                                player.Dex = 3;
                                player.Hp = 100;
                                player.Gold = 1500;
                                break;
                            case Job.Mage:
                                player.Job = "마법사";
                                player.Level = 1;
                                player.Atk = 5;
                                player.Def = 5;
                                player.Wis = 20;
                                player.Dex = 3;
                                player.Hp = 100;
                                player.Gold = 1500;
                                break;
                            case Job.Archer:
                                player.Job = "궁수";
                                player.Level = 1;
                                player.Atk = 3;
                                player.Def = 4;
                                player.Wis = 3;
                                player.Dex = 20;
                                player.Hp = 100;
                                player.Gold = 1500;
                                break;
                        }

                        player.DefaultEquip();

                        Town();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\n\n\n\n\n\n\n");
                        Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                        selectedJob = Job.Rechoice;
                    }
                }
                while (selectedJob == Job.Rechoice);
                {
                    return selectedJob;
                }
        }

        // enum 직업
        public enum Job
        {
            Rechoice = 0,
            Warrior = 1,
            Mage = 2,
            Archer = 3
        }

        // 입력값 확인하는
        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                    
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("\n\n\n");
                Console.WriteLine("잘못된 입력입니다.");
            }
        }

        // #4. 이어하기 (X)
        static void Load()
        {
            Console.WriteLine("이어하기 기능은 아직 구현되지 않았습니다.");
        }

        // #5. 종료하기
        static void End()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("게임이 종료됩니다. 다음에 또 만나요!");
            Environment.Exit(0);
        }

        // #6. 내 정보
        static void MyInfo()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("캐릭터의 정보를 표시합니다.");
            Console.WriteLine();
            Console.WriteLine($"Lv.{player.Level}");
            Console.WriteLine($"{player.Name}");
            Console.WriteLine($"{player.Job}");
            Console.WriteLine($"공격력 :{player.TotalAtk}");
            Console.WriteLine($"방어력 : {player.TotalDef}");
            Console.WriteLine($"마법공격력 : {player.TotalWis}");
            Console.WriteLine($"공격속도 : {player.TotalDex}");
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.TotalGold} G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 0);

            switch (input)
            {
                case 0:
                    Town();
                    break;
            }
        }

        // #7. 인벤토리
        static void Inventory()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("\n");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("\n");

            foreach (Item item in player.Inventory)
            {
                string equippedSign = item.IsEquipped ? "[E]" : "";
                Console.WriteLine($"{equippedSign} {item.ItemName} | 공격력 + {item.ItemAtk} | 방어력 + {item.ItemDef} | 마법공격력 + {item.ItemWis} | 공격속도 + {item.ItemDex} | {item.ItemDescription}");
            }

            Console.WriteLine($"보유 골드 : {player.TotalGold} G");
            Console.WriteLine("\n");
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 1);
                switch (input)
                {
                    case 1:
                        EquipSetting();
                        break;
                    case 0:
                        Town();
                        break;
                }
        }
        
        // 인벤토리 - 장착 관리
        static void EquipSetting()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("장착할 아이템을 선택하세요.");
            Console.WriteLine("\n");

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                string equippedSign = player.Inventory[i].IsEquipped ? "[E]" : "";
                Console.WriteLine($"[{i + 1}] {equippedSign}{player.Inventory[i].ItemName}");
            }

            Console.WriteLine("\n");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, player.Inventory.Count);

            if (input == 0)
            {
                Inventory();
            }
            else
            {
                Item selectItem = player.Inventory[input - 1];
                player.EquipItem(selectItem);

                Console.WriteLine($"{selectItem.ItemName} {(selectItem.IsEquipped ? "장착 완료" : "해제 완료")}!");
                Console.ReadKey();
                EquipSetting();
            }
        }

        // #8. 상점
        static void Store()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("어서오세요, 스파르타 상점입니다.");
            Console.WriteLine("무엇을 하시겠어요?");
            Console.WriteLine("\n");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, 2);
            
            switch (input)
            {
                case 1:
                    StoreBuy();
                    break;
                case 2:
                    StoreSell();
                    break;
                case 0:
                    Town();
                    break;
            }
        }

        // # 상점 - 구매
        static void StoreBuy()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("원하는 아이템을 골라보세요!");
            Console.WriteLine("\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.TotalGold} G");
            Console.WriteLine("\n");
            Console.WriteLine("[아이템 목록]");

            int i = 0;
            foreach (Item item in items)
            {
                string BuySign = player.Inventory.Contains(item) ? "보유 아이템" : "";
                Console.WriteLine($"[{i + 1}] {item.ItemName} | 공격력 + {item.ItemAtk} | 방어력 + {item.ItemDef} | 마법공격력 + {item.ItemWis} | 공격속도 + {item.ItemDex} \n {item.ItemDescription} | {item.ItemGold} G [{BuySign}]");
                i++;
            }

            Console.WriteLine("\n");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, i);
            
            if (input == 0)
            {
                Store();
            }
            else
            {

                Item buyItem = items[input - 1];
                if (player.Gold >= buyItem.ItemGold && !player.Inventory.Contains(buyItem))
                {
                    player.ItemBought(buyItem);
                    player.Inventory.Add(buyItem);

                    Console.WriteLine($"{buyItem.ItemName} 구매 완료!");
                }
                else if (player.Gold >= buyItem.ItemGold && !player.Inventory.Contains(buyItem))
                {
                    Console.WriteLine("이미 보유하고 계시네요!");
                }
                else if (player.Gold <= buyItem.ItemGold)
                {
                    Console.WriteLine("골드가 부족해요 :(");
                }
                Console.ReadKey();
                StoreBuy();
            }

        }

        // # 상점 - 판매
        static void StoreSell()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine("판매하고 싶은 아이템을 골라주세요!");
            Console.WriteLine("\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.TotalGold} G");
            Console.WriteLine("\n");
            Console.WriteLine("[아이템 목록]");

            int i = 0;
            foreach (Item item in player.Inventory)
            {
                string equippedSign = player.Inventory[i].IsEquipped ? "[E]" : "";
                Console.WriteLine($"{equippedSign} {item.ItemName} | 공격력 + {item.ItemAtk} | 방어력 + {item.ItemDef} | 마법공격력 + {item.ItemWis} | 공격속도 + {item.ItemDex} | {item.ItemGold} G");
                i++;
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            int input = CheckValidInput(0, i);

            if (input == 0)
            {
                Store();
            }
            else
            {
                Item sellItem = items[input - 1];
                if (player.Inventory.Contains(sellItem))
                {
                    player.ItemSold(sellItem);
                    player.Inventory.Remove(sellItem);

                    Console.WriteLine($"{sellItem.ItemName} 판매 완료!");
                }
                else
                {

                }
                Console.ReadKey();
                StoreSell();
            }
        }

        // #9. 휴식 (X)
        static void Rest()
        {
            Console.Clear();
        }

        // #10. 던전 입장 (X)
        static void EnterDungeon()
        {
            Console.Clear();
        }

        // #11. 저장 (X)
        static void Save()
        {
            Console.Clear();
        }

        // #12. ?

        // #13. 아이템 리스트
        private static Item[] items = new Item[]
        {
            //기본 스파르타 세트
            new Item("스파르타 갑옷", 0, 3, 0, 0, "스파르타의 보급형 갑옷", 1) { EquipAll = -1 }, // 0
            new Item("스파르타 하의", 0, 2, 0, 0, "스파르타의 보급형 하의", 1) { EquipAll = -1 }, // 1
            new Item("스파르타 장갑", 0, 1, 0, 1, "스파르타의 보급형 장갑", 1) { EquipAll = -1 }, // 2
            new Item("스파르타 신발", 0, 2, 0, 0, "스파르타의 보급형 신발", 1) { EquipAll = -1 }, // 3
            new Item("스파르타 모자", 0, 2, 0, 0, "스파르타의 보급형 모자", 1) { EquipAll = -1 }, // 4
            new Item("스파르타 검", 3, 0, 0, 0, "스파르타의 보급형 검", 3) { EquipJob = (int)Job.Warrior}, // 5
            new Item("스파르타 지팡이", 0, 0, 3, 0, "스파르타의 보급형 지팡이", 3) { EquipJob = (int)Job.Mage}, // 6
            new Item("스파르타 활", 0, 0, 0, 3, "스파르타의 보급형 활", 3) { EquipJob = (int)Job.Archer}, // 7

            //신트라 세트
            new Item("신트라 갑옷", 0, 5, 0, 0, "신트라에서 수입한 갑옷", 100) { EquipAll = -1 }, // 8
            new Item("신트라 하의", 0, 4, 0, 0, "신트라에서 수입한 하의", 100) { EquipAll = -1 }, // 9
            new Item("신트라 장갑", 0, 3, 0, 0, "신트라에서 수입한 장갑", 100) { EquipAll = -1 }, // 10
            new Item("신트라 신발", 0, 4, 0, 0, "신트라에서 수입한 신발", 100) { EquipAll = -1 }, // 11
            new Item("신트라 모자", 0, 4, 0, 0, "신트라에서 수입한 신발", 100) { EquipAll = -1 }, // 12
            new Item("신트라 검", 5, 0, 0, 0, "신트라 검이면 믿고 씁니다!", 800) { EquipAll = (int)Job.Warrior }, // 13
            new Item("신트라 지팡이", 0, 0, 5, 0, "신트라 지팡이? 좋은건가?", 800) { EquipAll = (int)Job.Mage }, // 14
            new Item("신트라 활", 0, 0, 0, 5, "신트라에서 활도 수입하나?", 800) { EquipAll = (int)Job.Archer }, // 15

            //늑대 교단 세트
            new Item("늑대 교단 갑옷", 0, 10, 0, 0, "늑대 교단의 전사들이 착용했다는 전설의 갑옷", 100) { EquipAll = -1 }, // 16
            new Item("늑대 교단 하의", 0, 8, 0, 0, "늑대 교단의 전사들이 착용했다는 전설의 하의", 100) { EquipAll = -1 }, // 17
            new Item("늑대 교단 장갑", 0, 6, 0, 0, "늑대 교단의 전사들이 착용했다는 전설의 장갑", 100) { EquipAll = -1 }, // 18
            new Item("늑대 교단 신발", 0, 8, 0, 0, "늑대 교단의 전사들이 착용했다는 전설의 신발", 100) { EquipAll = -1 }, // 19
            new Item("늑대 교단 모자", 0, 8, 0, 0, "늑대 교단의 전사들이 착용했다는 전설의 모자", 100) { EquipAll = -1 }, // 20
            new Item("게롤트의 은검", 10, 0, 0, 0, "모든 전사들이 부러워할 전설의 검", 800) { EquipAll = (int)Job.Warrior }, // 21

            //코비어 세트
            new Item("코비어 마법사의 로브", 0, 7, 0, 0, "코비어의 대장장이도 마법사라는 소문이 있던데..", 100) { EquipAll = -1 }, // 22
            new Item("코비어 마법사의 하의", 0, 6, 0, 0, "코비어의 대장장이도 마법사라는 소문이 있던데..", 100) { EquipAll = -1 }, // 23
            new Item("코비어 마법사의 장갑", 0, 5, 0, 0, "코비어의 대장장이도 마법사라는 소문이 있던데..", 100) { EquipAll = -1 }, // 24
            new Item("코비어 마법사의 신발", 0, 6, 0, 0, "코비어의 대장장이도 마법사라는 소문이 있던데..", 100) { EquipAll = -1 }, // 25
            new Item("코비어 마법사의 모자", 0, 6, 0, 0, "코비어의 대장장이도 마법사라는 소문이 있던데..", 100) { EquipAll = -1 }, // 26
            new Item("트리스의 귀걸이", 0, 0, 10, 0, "트리스의 힘이 깃든 귀걸이", 800) { EquipAll = (int)Job.Mage }, // 27
            
            //스코이아텔 세트
            new Item("스코이아텔 엘프의 갑옷", 0, 6, 0, 0, "얇지만, 가볍습니다", 100) { EquipAll = -1 }, // 28
            new Item("스코이아텔 엘프의 하의", 0, 5, 0, 0, "얇지만, 가볍습니다", 100) { EquipAll = -1 }, // 29
            new Item("스코이아텔 엘프의 장갑", 0, 6, 0, 0, "얇지만, 가볍습니다", 100) { EquipAll = -1 }, // 30
            new Item("스코이아텔 엘프의 신발", 0, 6, 0, 0, "얇지만, 가볍습니다", 100) { EquipAll = -1 }, // 31
            new Item("스코이아텔 엘프의 모자", 0, 5, 0, 0, "얇지만, 가볍습니다", 100) { EquipAll = -1 }, // 32
            new Item("이오베스의 활", 0, 0, 0, 10, "모든 궁수들이 원하는 이오베스의 활", 800) { EquipAll = (int)Job.Archer }, // 33
        };

        //마을
        static void Town()
        {
            Console.Clear();

            Console.WriteLine("\n\n\n\n\n");
            Console.WriteLine(player.Name + "님, 스파르타 마을에 오신 것을 환영해요!");
            Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있어요.");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 휴식");
            Console.WriteLine("5. 던전 입장");
            Console.WriteLine("6. 저장");
            Console.WriteLine("0. 종료");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            int input = CheckValidInput(0, 6);
            switch (input)
            {
                case 1:
                    MyInfo();
                    break;

                case 2:
                    Inventory();
                    break;

                case 3:
                    Store();
                    break;

                case 4:
                    Rest();
                    break;

                case 5:
                    EnterDungeon();
                    break;

                case 6:
                    Save();
                    break;

                case 0:
                    Console.Clear();
                    Console.WriteLine("\n\n\n\n\n\n\n");
                    Console.WriteLine("정말 종료하시겠습니까?");
                    Console.WriteLine("[0] 종료");
                    Console.WriteLine("[1] 돌아가기");
                   
                    if (int.TryParse(Console.ReadLine(), out int reallyEnd))
                    {
                        if (reallyEnd == 0)
                        {
                            End();
                        }
                        else
                        {
                            Town();
                        }
                    }
                    else
                    {
                        Console.WriteLine("올바른 숫자를 입력하세요.");
                        Town();
                    }
                break;
            }
        }

        // 메인
        static void Main(string[] args)
        {
            ConsoleSetting();
           
            do
            {
                int select;
                   
                TitleDraw();
                   
                Console.WriteLine("\n\n\n");
                Console.WriteLine("                                       [1] 새로하기 ");
                Console.WriteLine("                                       [2] 이어하기 ");
                Console.WriteLine("                                       [3] 종료하기 ");
                int firstSelcet = int.Parse(Console.ReadLine());
                Console.WriteLine("\n\n\n");
               
                Console.Clear();

                switch (firstSelcet)
                {
                    case 1:
                        Getname();
                        break;

                    case 2:
                        Load();
                        break;

                    case 3:
                        End();
                        break;
                }
            }
            while (true);
            {

            }

        }
    }
}