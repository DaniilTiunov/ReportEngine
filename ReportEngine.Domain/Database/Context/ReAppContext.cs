using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;

namespace ReportEngine.Domain.Database.Context;

public class ReAppContext : DbContext
{
    public ReAppContext(DbContextOptions<ReAppContext> options) : base(options)
    {
    }

    //Сортаменты
    public DbSet<CarbonPipe> CarbonPipes { get; set; } //Сортамент труб Труба углеродистая сталь
    public DbSet<HeaterPipe> HeaterPipes { get; set; } //Труба нержавеющая сталь
    public DbSet<StainlessPipe> StainlessPipes { get; set; } //Труба жарапрочная сталь
    public DbSet<CarbonArmature> CarbonArmatures { get; set; } //Запорные арматуры Запорная арматура углеродистая сталь
    public DbSet<HeaterArmature> HeaterArmatures { get; set; } //Запорная арматура жарапрочная сталь
    public DbSet<StainlessArmature> StainlessArmatures { get; set; } //Запорная арматура нержавеющая сталь
    public DbSet<CarbonSocket> CarbonSockets { get; set; } //Тройники и КМЧ углеродистая сталь
    public DbSet<HeaterSocket> HeaterSockets { get; set; } //Тройники и КМЧ жарапрочная сталь
    public DbSet<StainlessSocket> StainlessSockets { get; set; } //Тройники и КМЧ нержавеющая сталь
    public DbSet<Drainage> Drainages { get; set; } //Дренаж
    public DbSet<FrameDetail> FrameDetails { get; set; } //Рамные комплектующие Детали рамы
    public DbSet<PillarEqiup> PillarEqiups { get; set; } //Комплектующие для стойки
    public DbSet<FrameRoll> FrameRolls { get; set; } //Прокат
    public DbSet<SensorBrace> SensorsBraces { get; set; } //Кронштейны Крепление датчиков
    public DbSet<DrainageBrace> DrainageBraces { get; set; } //Крепление дренажа
    public DbSet<BoxesBrace> BoxesBraces { get; set; } //Крепление клеммных коробок
    public DbSet<CabelBoxe> CabelBoxes { get; set; } //Клеммные коробки
    public DbSet<CabelInput> CabelInputs { get; set; } //Кабельные вводы
    public DbSet<CabelProduction> CabelProductions { get; set; } //Кабельная продукция
    public DbSet<CabelProtection> CabelProtections { get; set; } //Средства прокладки и защиты кабеля
    public DbSet<Heater> Heaters { get; set; } //Обогрев
    public DbSet<Other> Others { get; set; } //Прочие   
    public DbSet<Container> Containers { get; set; } //Тара

    //Обвязки
    public DbSet<Obvyazka> Obvyazki { get; set; } //Обвязки

    public DbSet<ObvyazkaInStand> ObvyazkiInStands { get; set; } // Обвязки в стендах

    //Сформированные рамы
    public DbSet<FormedFrame> FormedFrames { get; set; } //Сформированные рамы
    public DbSet<FrameComponent> FrameComponents { get; set; } //Компоненты рамы

    public DbSet<StandFrame> StandFrames { get; set; }

    //Сформированные дренажи
    public DbSet<FormedDrainage> FormedDrainages { get; set; }
    public DbSet<DrainagePurpose> DrainagePurposes { get; set; }

    public DbSet<StandDrainage> StandDrainages { get; set; }

    // Электрические компоненты
    public DbSet<FormedElectricalComponent> FormedElectricalComponents { get; set; }
    public DbSet<ElectricalPurpose> ElectricalPurposes { get; set; }

    public DbSet<StandElectricalComponent> StandElectricalComponents { get; set; }

    // Дополнительные комплектующие
    public DbSet<FormedAdditionalEquip> FormedAdditionalEquips { get; set; }
    public DbSet<AdditionalEquipPurpose> AdditionalEquipPurposes { get; set; }
    public DbSet<StandAdditionalEquip> StandAdditionalEquips { get; set; }

    //Остальные сущности
    public DbSet<ProjectInfo> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Stand> Stands { get; set; }

    public DbSet<ContainerStand> ContainersStand { get; set; } // Ящики с стендами
    public DbSet<ContainerBatch> ContainersBatch { get; set; } // Ящики с стендами
}