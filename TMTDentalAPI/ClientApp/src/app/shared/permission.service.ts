import { Injectable, EventEmitter } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class PermissionService {
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
        return permissions.some(v => {
            if (typeof v === "string")
                return this._permissionStore.indexOf(v.toLowerCase()) >= 0;
        });
    }

    public hasDefined(permission: string): boolean {
        if (typeof permission !== "string")
            return false;
        let index = this._permissionStore.indexOf(permission.toLowerCase());
        return index > -1;
    }

    clearStore() {
        this._permissionStore = [];
    }
}