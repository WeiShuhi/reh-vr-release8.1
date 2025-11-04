using ET;

namespace ET.Client
{
    /// <summary>
    /// 登录成功事件
    /// </summary>
    public class LoginSuccessEvent : Entity
    {
        public string Account { get; set; }
        public string Password { get; set; }
        
        public override void Dispose()
        {
            base.Dispose();
            this.Account = null;
            this.Password = null;
        }
    }
    
    /// <summary>
    /// 登出事件
    /// </summary>
    public class LogoutEvent : Entity
    {
        public string Account { get; set; }
        
        public override void Dispose()
        {
            base.Dispose();
            this.Account = null;
        }
    }
}