using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pcbaoi.Tools
{
    class StateMachine
    {
        public enum States
        {
            None,
            Logined,
            WaitingFor,
            
            Program_Ready,
            Program_Start,

            Program_Camera_Ready,
            Program_Camera_Runing,
            Program_Cmaera_Completed,
            
            Programming,
            Program_Completed,
            Program_Test_Ready,

            Program_Test_Completed,

            Work_Ready,
            Work_Start,
            Work_Camera_Ready,
            Work_Camera_Runing,
            Work_Camera_Completed,
            Work_Done,

            Error,
            Stoped,
            Closed
        }

        public enum Events
        {
            OnLogin,
            OnAutoCkeck,
            OnNewProject,
            OnTest,
            OnPrepare,
            OnWork,
            OnStart,
            OnLoad,
            OnCmaeraComplete,
            OnRedo,
            OnStop,
            OnResume,
            OnError
        }



        public static PassiveStateMachine<States, Events> machine;

        public static void ini()
        {
            var builder = new StateMachineDefinitionBuilder<States, Events>();
            /*
             *  None: The state enters into its initial sub state. The sub state itself enters its initial sub state and so on until the innermost nested state is reached.
                Deep: The state enters into its last active sub state. The sub state itself enters into its last active state and so on until the innermost nested state is reached.
                Shallow: The state enters into its last active sub state. The sub state itself enters its initial sub state and so on until the innermost nested state is reached.
             */
            builder.DefineHierarchyOn(States.Program_Ready) // 准备编程
                .WithHistoryType(HistoryType.Shallow)
                .WithInitialSubState(States.Program_Start) // 开始编程
                .WithSubState(States.Program_Camera_Ready) // 准备相机
                .WithSubState(States.Program_Camera_Runing) // 拍摄
                .WithSubState(States.Program_Cmaera_Completed) // 拍摄完成
                .WithSubState(States.Program_Completed); // 编程完成

            builder.DefineHierarchyOn(States.Program_Test_Ready) // 准备测试
                .WithHistoryType(HistoryType.Deep)
                .WithInitialSubState(States.Work_Ready) // 准备运行
                .WithSubState(States.Program_Test_Completed); // 测试完成

            builder.DefineHierarchyOn(States.Work_Ready) // 准备运行
                .WithHistoryType(HistoryType.Shallow)
                .WithInitialSubState(States.Work_Start) // 开始运行
                .WithSubState(States.Work_Camera_Ready) // 准备相机
                .WithSubState(States.Work_Camera_Runing) // 拍摄
                .WithSubState(States.Work_Camera_Completed) // 相机拍摄完成
                .WithSubState(States.Work_Done); // 一次运行完成
            

            builder.In(States.WaitingFor) // 登录完成后
                .On(Events.OnNewProject).Goto(States.Program_Ready) // 准备编程
                .On(Events.OnWork).Goto(States.Work_Ready); // 准备运行

            builder.In(States.Program_Completed) // 编程完成后
                .On(Events.OnTest).Goto(States.Program_Test_Ready) // 准备测试
                .On(Events.OnWork).Goto(States.Work_Ready); // 准备运行

            #region 异常 停止 恢复
            builder.In(States.Program_Ready)
                .On(Events.OnError).Goto(States.Error)
                .On(Events.OnStop).Goto(States.Stoped)
                .On(Events.OnResume).Goto(States.Program_Ready);

            builder.In(States.Program_Test_Ready)
                .On(Events.OnError).Goto(States.Error)
                .On(Events.OnStop).Goto(States.Stoped)
                .On(Events.OnResume).Goto(States.Program_Test_Ready);

            builder.In(States.Work_Ready)
                .On(Events.OnError).Goto(States.Program_Ready)
                .On(Events.OnStop).Goto(States.Stoped)
                .On(Events.OnResume).Goto(States.Work_Ready);
            #endregion

            builder.In(States.None)
                //.ExecuteOnEntry(logineddddddd)
                //.ExecuteOnExit(logineddddddd)
                //.ExecuteOnExit(Beep) // just beep a second time

                .On(Events.OnLogin).Goto(States.Logined).Execute(logineddddddd);
                 //.On(Events.OnLogin).If<string>(Login.translation).Goto(States.Logined).Execute(logineddddddd)
                 //.Otherwise().Execute(logine2ddddddd);
                //.On(Events.OnAutoCkeck)
                //    .If().Goto(States.WaitingFor)
                //    .Otherwise().Goto(States.Logined).Execute(this.AnnounceCheckFail);

 

            builder.WithInitialState(States.None);

            var definition = builder
                .Build();

            machine = definition
                .CreatePassiveStateMachine("Elevator");

            machine.Start();
        }
        private static void logineddddddd()
        {
            MessageBox.Show("登录成功");
        }
        private static void logine2ddddddd()
        {
            MessageBox.Show("登录失败");
        }

        /// <summary>
        /// 自检异常
        /// </summary>
        private void AnnounceCheckFail()
        {

        }

        private bool CheckOverload()
        {
            return false;
        }

    }
}
