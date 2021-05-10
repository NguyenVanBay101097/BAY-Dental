import { Injectable } from "@angular/core";

@Injectable({
    providedIn: "root",
})
export class CheckPermissionService {
    constructor() {}

    check(permission) {
        debugger;
        const pm = localStorage.getItem("user_permission");
        const user_permission = JSON.parse(pm);
        if (user_permission) {
            if (user_permission.isUserRoot) {
                return true;
            }
            if (user_permission.permission) {
                return permission.some(x => {
                    return user_permission.permission.includes(x);
                });
            }
        }
        return false;
    }
}
