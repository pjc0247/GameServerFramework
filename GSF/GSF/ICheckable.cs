using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF
{
    public interface ICheckable
    {
        /// <summary>
        /// 이 메소드를 상속하여, 이 오브젝트가 계속 유지되어도 되는지 설정한다.
        /// </summary>
        /// <returns>true일 경우 유지, false 일 경우 종료</returns>
        bool OnHealthCheck();

        /// <summary>
        /// 이 메소드를 상속하여 오브젝트를 정리하는 방법을 정의한다.
        /// </summary>
        /// <remarks>
        /// [DON`T] 소멸자 용도로 쓰지 말것
        /// (헬스체크 이외의 이유로 종료된 오브젝트는 OnDispose가 실행되지 않는다)
        /// </remarks>
        void OnDispose();
    }
}
