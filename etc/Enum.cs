public enum SystemMsgType
{
    OK,
    Cancel,
    Guidance,
    Quit,
    Loading,
    Login,
    Login_GameCenter,    // **Apple로 변경 필요
    Login_Guest,
    Patch,               
    Payment,             
    Login_Google,
    Login_Facebook,
    Term_Use,
    Term_GameService,
    Term_PersonalInfo,
    Agree,
    Server_Ditne,
    Server_Lakstia,
    SeverSelect,
    GameStart,
}


#region Hero Info

public enum RaceType
{
    All = -1,
    Human = 0,
    Elf,
    Angel,
    Demon,
    // Vampire  : 안씀 4
    Kemono = 5,
    Dragon,

    // Goblin : 안씀 7
}

public enum ClassType
{
    All = -1,
    Dealer = 0,
    Supporter,
    Tanker,
}

public enum PartyUIType
{
    Party,          // 파티구성
    Book,     // 영웅도감
    Kizuna          // 인연
}

public enum PartyEffectType
{
    None = -1,
    Race = 0,
    Class,
    Kizuna,
    Apply
}

public enum HeroPartyType
{
    InGame,
    Colosseum,
    Tower,
}

public enum HeroSortingType
{
    Level,
    Grade,
    Awaken,
}

#endregion

#region Battle

// 스킬사용 타겟
public enum SkillTargetType
{
    None = -1,
    Myself = 0, // 자신
    Team,       // 아군
    TeamAll,    // 파티 전체
    Enemy,      // 적
    EnemyAll    // 적 전체
}

public enum BattleResult
{
    Unknown = 0,
    Success = 1,
    Fail = 2
}

public enum DamageFontType
{
    None,
    Damage,
    Heal,
    Critical,
    // 그 이상...
    Miss,
}

public enum PhaseStep
{
    None,       // 넘기기
    Battle,     // 전투
    Move,       // 이동

    End,        // Excel 내에는 없음
}


#endregion

#region MenuType

public enum BottomBackgroundType
{
    None = 0,
    HeroAndFairy = 1,
    Equip = 2,
    Challenge = 3,
    ColosseumInfo =4,

    TowerInfo = 33
}

// **공식적인 영문명 있으면 변경해야함
public enum TempleType
{
    None = -1,
    Piers = 0,  // 피어스
    Yuria,      // 유리아
    Seize,      // 시즈
    Rea         // 레아
}

public enum ChallengeType
{
    Daily,      // 요일던전
    Boss,       // 보스던전
    Tower,      // 시련의 탑
    Arena,      // 결투장
    Explore,    // 탐색
}

#endregion MenuType

public enum ArtifactTargetType
{
    Race,       // 종족
    Type,       // 직업
    TableNum,   // 특정 영웅
    All = 4,    // 전체
}

public enum ShaderType
{
    Normal,
    GrayScale
}

public enum TwoButtonPopupType
{
    Normal, Rebirth, Sell
}

#region Item

public enum ItemType
{
    None = -1,
    Goods = 0,
    HeroPiece,
    Artifact,
    GameSpeed,
    Treasure,
    Exp,
    Gacha,
    AutoSkill

}

public enum ChargedItemType
{
    ColosseumTicket,
    ColosseumRefresh,
    RaidTicket = 2,
}

public enum PaymentType
{
    Diamond,                // 구매한 캐쉬 : 다이아
    FreeCash,               // 지급받은 캐쉬
    Gold,                   // 골드
    RebirthStone,           // 환생석
    AttackStone,            // 공격형 승급석
    DefenceStone,           // 방어형 승급석
    SupportStone,           // 지원형 승급석
    Crystal,                // 크리스탈
    StorySkipTicket,        // 스토리 스킵 - 보스 바로 도전권
    Scroll = 10,            // 유물 각성, 합성 스크롤
    Medal = 13,             // 신전재료
}

public enum AccelerationType 
{
    Double,
    Triple,
}

public enum TreasureType
{
    Bronze = 14,
    Sliver,
    Gold,
}

public enum GachaRewardType
{
    HeroPiece,
    Hero,
    Artifact,
}


#endregion Item

#region store

public enum MarketType
{
    Google,
    Apple,
    OneStore
}

public enum StoreType
{
    None = -1,
    CashShop = 0,   // 유료상점     **No Language
    DiaShop,        // 다이아상점   **No Language
    Exchanger,      // 교환소
}

public enum CashTabs
{
    Dia,
    Pass,
}

public enum DiaTabs
{
    Package,
    Gold,
    Gacha,
    Item
}

public enum ExchangerTabs
{
    Ordeal
}

public enum BuyType
{
    Ads = -2,   // 광고
    Cash = -1,  // 유료구매
    Goods      // 재화구매
}

#endregion


#region Tutorial

public enum TutorialTrackingType
{
    Attendance = 0,     // 출석부
    Fairy_LevelUp,
    Party_Add,
    Hero_LevelUp,
    BossStage_Enter,
    BossStage_Clear,
    Hero_Gacha,
    Party_Change,
    Unit_LevelUp_Change,    // 레벨업 단위 변경
    Temple,
    Rebirth_Start,
    Rebirth_End,
    Resurrection_LevelUp,    // 특성
    Achieve,
    Hero_Gacha11,
}

#endregion Tutorial

#region Help

public enum HelpInfoType
{
    Grade,
    Awaken,
    Dungeon,        // 요일던전
    Raid,           // 보스전
    ColosseumRule,
    ColosseumReward,
    BossDungeon,    // 보스던전
    ArtifactReinforce,
    ArtifactAwaken,
    ArtifactCompose,
}

#endregion Help

#region Episode

public enum DialogueTalkerPos
{   // 말하는 사람의 위치
    Left, Right, None,
}

public enum DialogueType
{
    Typing, Once, None
}

public enum HSceneType
{
    Normal,
    Fast,
    Finish
}

#endregion Episode

#region Item연동 

public enum AchieveType
{
    Daily,
    Weekly,
    Normal
}

public enum AchievementType
{
    Resurrection = 8,
    HeroCount,
    HeroAwaken4Count,
    Level = 15,
    SumHeroGrade = 29,
    HeroAwaken5Count = 30
}

public enum RebirthSkillType
{
    Normal,
    Premium,
}


public enum RankingType
{
    Stage,
    Rebirth,
    Colosseum,
}

public enum ArtifactType
{
    Enchant,
    Awaken,
    Compose,
}

public enum MyInfoType
{
    Name,
    Level,
    Score,
    Rank,
}


public enum InventoryItemType
{
    Artifact = 0,
    Piece,
    Normal,
    Ticket,
}

public enum ArtifactResultType
{
    AwakenSuccess,
    AwakenFail,
    ComposeSuccess,
    ComposeFail,

}

//public enum DungeonDifficultyType
//{
//    Attack,
//    Defence,
//    Support
//}

#endregion Item연동