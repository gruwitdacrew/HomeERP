﻿using HomeERP.Models.Domain;

namespace HomeERP.Models.DTOs.Response
{
    public class EntityCollectionResponse
    {
        public EntityWideResponse? CurrentEntity { get; set; }

        public List<Entity> Entities { get; set; }

        public EntityCollectionResponse(EntityWideResponse? CurrentEntity, List<Entity> Entities)
        {
            this.CurrentEntity = CurrentEntity;
            this.Entities = Entities;
        }
    }
}
