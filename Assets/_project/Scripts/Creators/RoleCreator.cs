using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CharacterCreation
{
    public class RoleCreator : DataCreator, IDataCreator
    {
        private List<RoleData> _roles = new();
        public List<RoleData> Roles => _roles;

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            string basePath = Path.Combine(Application.streamingAssetsPath, "Роли");
            var task = LoadAndAddAsync<List<RoleData>, RoleData>(
                "Роли.json",
                _roles,
                cancellationToken,
                list => list,
                basePath);
            await UniTask.WhenAll(task);
        }
    }

    [Serializable]
    public class RoleData
    {
        public string roleName;
        public string description;
        public int amountTalents;
        public int specializationAmount;
        public List<string> talents;
        public SkillUpgrade skill_upgrades;
        public List<string> specialization;
        public List<List<string>> equipments;
    }
}

