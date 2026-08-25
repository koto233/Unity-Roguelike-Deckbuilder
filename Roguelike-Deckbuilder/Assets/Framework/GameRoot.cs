using System.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.Audio;
using LitFramework.Config;
using LitFramework.FSM;
using LitFramework.FSM.Procedure;
using LitFramework.ObjectPool;
using LitFramework.UI.Core.Service;
using UnityEngine;
namespace LitFramework
{
    /// <summary>
    /// 游戏根对象，负责管理全局服务和游戏状态
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }
        [SerializeField] private Canvas _uiRoot;
        private ProcedureManager _procedureManager;
        public Canvas UIRoot => _uiRoot;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        void Start()
        {

        }
        void Update()
        {
            _procedureManager.Update();
        }


        private void Init()
        {
            InitGameServices();
            InitUI();
            InitProcedure();
        }
        private void InitGameServices()
        {
            ServiceLocator.Register(new ObjectPoolService());
            ServiceLocator.Register<IConfigService>(new ConfigService());
            ServiceLocator.Register(new InputService());
            ServiceLocator.Register<IAudioService>(new AudioService());
            ServiceLocator.Register(new UIService());
            ServiceLocator.Register(new BattleInteractionService());
            ServiceLocator.Register<ISceneLoader>(new SceneLoader());
            ServiceLocator.Register(new MapService());
            ServiceLocator.Register(new SaveService(new JsonSaveStorage()));
            ServiceLocator.Register(new PlayerDataService());
            ServiceLocator.Register(new CardIconService());
            ServiceLocator.Register(new UIAtlasService());
            ServiceLocator.Register(new BattleController());
        }
        private void InitUI()
        {
            var uiService = ServiceLocator.Get<UIService>();
            uiService.Register<MainMenuView>(UIPath.MainMenuView, UILayer.Normal);
            uiService.Register<TopBar>(UIPath.TopBar, UILayer.Overlay);
            uiService.Register<SettingPanel>(UIPath.SettingPanel, UILayer.Popup);
            uiService.Register<BattleResultPanel>(UIPath.BattleResultPanel, UILayer.Normal);
            uiService.Register<BattleView>(UIPath.BattleView, UILayer.Normal);
            uiService.Register<MapView>(UIPath.MapView, UILayer.Normal);
            uiService.Register<DeckView>(UIPath.DeckView, UILayer.Normal);
            uiService.Register<RestView>(UIPath.RestView, UILayer.Normal);
            uiService.Register<ShopView>(UIPath.ShopView, UILayer.Normal);
            uiService.Register<EventView>(UIPath.EventView, UILayer.Normal);
            uiService.Bind<BattleResultPanel>(view => new BattleResultPresenter(view));
            uiService.Bind<MainMenuView>(view => new MainMenuPresenter(view));
            uiService.Bind<TopBar>(view => new TopBarPresenter(view));
            uiService.Bind<SettingPanel>(view => new SettingPresenter(view));
            uiService.Bind<BattleView>(view => new BattlePresenter(view));
            uiService.Bind<MapView>(view => new MapPresenter(view));
            uiService.Bind<DeckView>(view => new DeckPresenter(view));
            uiService.Bind<RestView>(view => new RestPresenter(view));
            uiService.Bind<ShopView>(view => new ShopPresenter(view));
            uiService.Bind<EventView>(view => new EventPresenter(view));

        }
        private void InitProcedure()
        {
            var fsm = new StateMachine();
            _procedureManager = new ProcedureManager(fsm);
            ServiceLocator.Register(_procedureManager);
            _procedureManager.RegisterProcedure(new ProcedureInit(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureTitle(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureBattle(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureMap(_procedureManager));
            // 监听状态变化（可选）
            fsm.OnStateChanged += (from, to) =>
            {
                Debug.Log($"流程状态变化: {from?.Name} → {to?.Name}");
            };
            _procedureManager.ChangeProcedure<ProcedureInit>();
            // _procedureManager.SetSharedData();
        }
    }

}
