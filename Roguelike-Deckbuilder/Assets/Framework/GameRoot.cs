using System.Threading.Tasks;
using LitFramework.Asset;
using LitFramework.FSM;
using UnityEngine;
namespace LitFramework
{
    /// <summary>
    /// 游戏根对象，负责管理全局服务和游戏状态
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }
        private ProcedureManager _procedureManager;
        public Canvas UIRoot { get; private set; }

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
            UIRoot = GameObject.Find("UIRoot").GetComponent<Canvas>();
            var fsm = new StateMachine();
            _procedureManager = new ProcedureManager(fsm);
            _procedureManager.RegisterProcedure(new ProcedureInit(fsm));
            _procedureManager.RegisterProcedure(new ProcedureHotFix(fsm));
            _procedureManager.RegisterProcedure(new ProcedureTitle(fsm));
            // 监听状态变化（可选）
            fsm.OnStateChanged += (from, to) =>
            {
                Debug.Log($"流程状态变化: {from?.Name} → {to?.Name}");
            };
            _procedureManager.StartProcedure<ProcedureInit>();
            // _procedureManager.SetSharedData();
        }


    }

}
