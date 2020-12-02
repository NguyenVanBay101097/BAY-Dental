import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DotKhamStepsOdataService } from 'src/app/shared/services/dot-kham-stepsOdata.service';
import { SaleOrdersOdataService } from "src/app/shared/services/sale-ordersOdata.service";

@Component({
  selector: "app-treatment-process-service-list",
  templateUrl: "./treatment-process-service-list.component.html",
  styleUrls: ["./treatment-process-service-list.component.css"],
})
export class TreatmentProcessServiceListComponent implements OnInit {
  saleOrderId: string;
  services: any;

  constructor(
    private route: ActivatedRoute,
    private saleOrderOdataService: SaleOrdersOdataService, 
    private dotKhamStepsOdataService: DotKhamStepsOdataService
  ) { }

  ngOnInit() {
    this.saleOrderId = this.route.queryParams['value'].id;

    if (this.saleOrderId) {
      this.saleOrderOdataService.getDotKhamStepByOrderLine(this.saleOrderId).subscribe(
        (result) => {
          console.log(result);
          this.services = result['value'];
        },
        (error) => { }
      );
    }
  }

  checkStatusDotKhamStep(step) {
    step.IsDone= !step.IsDone;
    var value = {
      Id: step.Id,
      IsDone: step.IsDone
    }
    this.dotKhamStepsOdataService.patch(step.Id, value).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => { }
    );
  }
}
