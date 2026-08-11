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
            ServiceLocator.Get<UIService>().Register<UITitleWindow>("Assets/Res/UI/UITitleWindow.prefab", UILayer.Normal);
            ServiceLocator.Get<UIService>().Register<UITopBar>("Assets/Res/UI/UITopBar.prefab", UILayer.Overlay);
            ServiceLocator.Get<UIService>().Register<UISetting>("Assets/Res/UI/UISetting.prefab", UILayer.Popup);
            ModelContainer.Register(new PlayerModel());

            var fsm = new StateMachine();
            _procedureManager = new ProcedureManager(fsm);
            ServiceLocator.Register(_procedureManager);
            // _procedureManager.RegisterProcedure(new ProcedureInitService(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureInitResource(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureInitConfig(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureTitle(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureBattle(_procedureManager));
            _procedureManager.RegisterProcedure(new ProcedureMap(_procedureManager));
            // 监听状态变化（可选）
            fsm.OnStateChanged += (from, to) =>
            {
                Debug.Log($"流程状态变化: {from?.Name} → {to?.Name}");
            };
            _procedureManager.ChangeProcedure<ProcedureInitResource>();
            // _procedureManager.SetSharedData();
        }


    }

}
