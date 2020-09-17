using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Volo.Abp.Collections;

namespace Volo.Abp.Data.Filter.Group
{
    public class ReqGroupList : List<IReqGroup>
    {
        public void AddGroup(IReqGroup group)
        {
            if (group.Rules == null || group.Rules.Count == 0)
            {
                return;
            }
            this.Add(group);
        }
    }
}
