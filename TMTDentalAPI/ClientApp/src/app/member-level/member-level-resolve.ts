import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MemberLevelService } from "./member-level.service";
@Injectable({
    providedIn: 'root'
})
export class MemberLevelResolve implements Resolve<any>{
    constructor(
        private memberLevelService: MemberLevelService,
        private router: Router
    ) { }
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.memberLevelService.get().pipe(
            catchError(error => {
                this.router.navigateByUrl('/404');
                return of(null);
            }
            ));
    }
}