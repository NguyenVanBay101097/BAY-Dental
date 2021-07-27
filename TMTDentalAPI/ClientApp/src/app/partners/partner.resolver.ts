import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable, of } from "rxjs";
import { PartnerDisplay } from "./partner-simple";
import { PartnerService } from "./partner.service";

@Injectable({ providedIn: 'root' })
export class PartnerResolver implements Resolve<PartnerDisplay> {
  constructor(private service: PartnerService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any>|Promise<any>|any {
    var id = route.queryParamMap.get('partner_id') || route.paramMap.get('id');
    if (!id) {
      return of(null);
    }

    return this.service.getCustomerInfo(id);
  }
}