using System.Collections;
using System.Collections.Generic;
using Game.CarSystem.Base;
using Game.LevelSystem.Controllers;
using Game.LevelSystem.Managers;
using Game.Managers;
using Game.SlingSystem.Managers;
using Game.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class VInstanller : LifetimeScope
{
    [SerializeField] private PoolManager _poolManager;
    [SerializeField] private OptLevelGenerator _levelGenerator;
    [SerializeField] private CarBase _carBase;
    [SerializeField] private PlayerView _playerView;


    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_poolManager);
        builder.RegisterComponent(_levelGenerator);
        builder.RegisterComponent(_carBase);
        builder.RegisterComponent(_playerView);


        builder.Register<AssetManager>(Lifetime.Singleton);
        builder.Register<LevelManager>(Lifetime.Singleton);
        builder.Register<SlingManager>(Lifetime.Singleton);


    }
}
