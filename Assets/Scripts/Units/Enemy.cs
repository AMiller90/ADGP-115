﻿using System;
using Library;
using UI;
using Units.Controller;
using UnityEngine;
using Event = Define.Event;

namespace Units
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : MonoBehaviour, IStats, IControlable
    {
        [SerializeField]
        private UnitNameplate m_Nameplate;

        [SerializeField]
        private NavMeshAgent m_NaveMeshAgent;
        private GameObject m_following;

        [SerializeField]
        private ControllerType m_ControllerType;
        [SerializeField]
        private IController m_Controller;

        [SerializeField]
        private FiniteStateMachine<MovementState> m_MovementFSM;

        [SerializeField]
        private FiniteStateMachine<DamageState> m_DamageFSM;

        [SerializeField]
        private string m_UnitName;
        [SerializeField]
        private string m_UnitNickname;
        [SerializeField]
        private float m_MaxHealth;
        [SerializeField]
        private float m_Health;
        [SerializeField]
        private float m_MaxDefense;
        [SerializeField]
        private float m_Defense;
        [SerializeField]
        private float m_MaxMana;
        [SerializeField]
        private float m_Mana;
        [SerializeField]
        private float m_Experience;  //Total experience each monster drops
        [SerializeField]
        private int m_Level; //wont be displayed for Enemy

        [SerializeField]
        private Vector3 m_TotalVelocity;
        [SerializeField]
        private Vector3 m_Velocity;
        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private Moving m_IsMoving;

        [SerializeField]
        private bool m_CanMoveWithInput;

        #region -- PROPERTIES --
        public ControllerType controllerType
        {
            get { return m_ControllerType; }
            set { m_ControllerType = value; }
        }
        public IController controller
        {
            get { return m_Controller; }
            set { m_Controller = value; }
        }

        public FiniteStateMachine<MovementState> movementFSM
        {
            get { return m_MovementFSM; }
            private set { m_MovementFSM = value; }
        }

        //Public string Name property
        public string unitName
        {
            get { return m_UnitName; }
            private set { m_UnitName = value; }
        }

        public string unitNickname
        {
            get { return m_UnitNickname; }
            set { m_UnitNickname = value; }
        }

        public float maxHealth
        {
            get { return m_MaxHealth; }
            private set { m_MaxHealth = value; }
        }
        //Public int health property
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.UnitHealthChanged, this); }
        }
        public float maxDefense
        {
            get { return m_MaxDefense; }
            private set { m_MaxDefense = value; }
        }
        //Public int Defense property
        public float defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }

        public float maxMana
        {
            get { return m_MaxMana; }
            private set { m_MaxMana = value; }
        }
        //Public int Mana property
        public float mana
        {
            get { return m_Mana; }
            set { m_Mana = value; Publisher.self.DelayedBroadcast(Event.UnitManaChanged, this); }
        }
        //Public int Experience property
        public float experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }
        //Public int Level property
        public int level
        {
            get { return m_Level; }
            set { m_Level = value; Publisher.self.DelayedBroadcast(Event.UnitLevelChanged, this); }
        }
        //Public int Speed property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }

        public Vector3 totalVelocity
        {
            get { return m_TotalVelocity; }
            set { m_TotalVelocity = value; }
        }
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public bool canMoveWithInput
        {
            get { return m_CanMoveWithInput; }
            set { m_CanMoveWithInput = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
        }

        public NavMeshAgent navMashAgent
        {
            get
            {
                return m_NaveMeshAgent;
            }
        }

        public GameObject following
        {
            get
            {
                return m_following;
            }

            set
            {
                m_following = value;
            }
        }
        #endregion

        void Awake()
        {
            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = this;
                nameplate.Awake();
            }

            if (m_NaveMeshAgent == null)
                m_NaveMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            SetController();

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);

            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        void Update()
        {
            if(m_Health <= 0.0f)
                Destroy(gameObject);

        }

        private void OnDestroy()
        {
            m_Controller.UnRegister(this);

            Publisher.self.Broadcast(Event.UnitDied, this);
        }

        private void SetController()
        {
            m_MovementFSM = new FiniteStateMachine<MovementState>();

            switch (m_ControllerType)
            {
                case ControllerType.Enemy:
                    m_Controller = AIController.self;
                    break;
                case ControllerType.Fortress:
                    m_Controller = UserController.self;
                    break;
                case ControllerType.User:
                    m_Controller = UserController.self;
                    break;
            }

            m_Controller.Register(this);
        }
    }
}
