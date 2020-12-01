import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  formGroup: FormGroup;
  saleOrderId: string;

  constructor(
    private route: ActivatedRoute,
    private saleOrderOdataService: SaleOrdersOdataService
  ) {}

  ngOnInit() {
    this.saleOrderId = this.route.snapshot.paramMap.get("id");
    debugger
    if (this.saleOrderId) {
      this.saleOrderOdataService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          console.log(result);
        },
        (error) => {}
      );
    }
  }
}
