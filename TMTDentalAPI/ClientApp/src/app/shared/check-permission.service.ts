import { Injectable } from "@angular/core";
import { SessionInfoStorageService } from "../core/services/session-info-storage.service";

@Injectable({
    providedIn: "root",
})
export class CheckPermissionService {
    constructor(private sessionInfoStorageService: SessionInfoStorageService) {}

    check(permissions: string[]) {
        const sessionInfo = this.sessionInfoStorageService.getSessionInfo();
        if (!sessionInfo) {
            return false;
        }

        if (sessionInfo.isAdmin) {
            return true;
        }

        return permissions.some(x => {
            return sessionInfo.permissions.includes(x);
        });
    }
}
