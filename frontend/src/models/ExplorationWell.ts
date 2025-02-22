import { DrillingSchedule } from "./assets/wellproject/DrillingSchedule"

export class ExplorationWell implements Components.Schemas.ExplorationWellDto {
    drillingSchedule?: DrillingSchedule
    explorationId?: string
    wellId?: string

    constructor(data?: Components.Schemas.ExplorationWellDto) {
        if (data !== undefined) {
            this.drillingSchedule = DrillingSchedule.fromJSON(data.drillingSchedule)
            this.explorationId = data.explorationId
            this.wellId = data.wellId
        } else {
            this.drillingSchedule = new DrillingSchedule()
        }
    }

    static fromJSON(data: Components.Schemas.ExplorationWellDto): ExplorationWell {
        return new ExplorationWell(data)
    }
}
