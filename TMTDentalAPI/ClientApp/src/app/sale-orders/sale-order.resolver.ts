import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { SaleOrderService } from "../core/services/sale-order.service";
import { SaleOrderBasic } from "./sale-order-basic";

@Injectable({ providedIn: 'root' })
export class SaleOrderResolver implements Resolve<SaleOrderBasic> {
  constructor(private service: SaleOrderService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any>|Promise<any>|any {
    var id = route.paramMap.get('id');
    return this.service.getBasic(id);
  }
}