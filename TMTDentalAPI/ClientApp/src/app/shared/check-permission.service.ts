import { Injectable } from "@angular/core";

@Injectable({
    providedIn: "root",
})
export class CheckPermissionService {
    constructor() {}

    check(permission) {
        const pm = localStorage.getItem("user_permission");
        const user_permission = JSON.parse(pm);
        if (user_permission) {
            if (user_permission.isUserRoot) {
                return true;
            }
            if (user_permission.permission) {
                return user_permission.permission.includes(permission);
            }
        }
        return false;
    }
}
