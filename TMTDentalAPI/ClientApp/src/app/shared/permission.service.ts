import { Injectable, EventEmitter } from "@angular/core";
import { SessionInfoStorageService } from "../core/services/session-info-storage.service";

@Injectable({ providedIn: 'root' })
export class PermissionService {

    constructor(private sessionInfoStorageService: SessionInfoStorageService) {}

    private _permissionStore: Array<string> = [];
    private _permissionStoreChange = new EventEmitter<any>();

    define(permissions: Array<string>) {
        if (!Array.isArray(permissions))
            throw "permissions parameter is not array.";

        this.clearStore();
        for (let permission of permissions)
            this.add(permission);
    }

    public add(permission: string): void {
        if (typeof permission === "string" && !this.hasDefined(permission.toLocaleLowerCase())) {
            this._permissionStore.push(permission.toLocaleLowerCase());
            this._permissionStoreChange.emit(null);
        }
    }

    get permissionStoreChangeEmitter(): EventEmitter<any> {
        return this._permissionStoreChange;
    }

    public hasOneDefined(permissions: Array<string>): boolean {
        if (!Array.isArray(permissions))
            throw "permissions parameter is not array.";
        const sessionInfo = this.sessionInfoStorageService.getSessionInfo();
        if (!sessionInfo) {
            return false;
        }
        
        return permissions.some(v => {
            if (typeof v === "string")
                return sessionInfo.groups.includes(v);
        });
    }

    public hasDefined(permission: string): boolean {
        if (typeof permission !== "string")
            return false;
        var groups = permission.split(',');
        return this.hasOneDefined(groups);
    }

    clearStore() {
        this._permissionStore = [];
    }
}