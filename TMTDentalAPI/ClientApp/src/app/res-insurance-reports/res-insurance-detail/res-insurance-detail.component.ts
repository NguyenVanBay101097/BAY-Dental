import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResInsuranceService } from 'src/app/res-insurance/res-insurance.service';

@Component({
  selector: 'app-res-insurance-detail',
  templateUrl: './res-insurance-detail.component.html',
  styleUrls: ['./res-insurance-detail.component.css']
})
export class ResInsuranceDetailComponent implements OnInit {
  id: string;
  insuranceInfo: any;
  insuranceName: any;
  constructor(
    public route: ActivatedRoute,
    private resInsuranceService: ResInsuranceService
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.getDisplayInsurance();
    }
  }

  getDisplayInsurance(): void {
    this.resInsuranceService.getById(this.id).subscribe((res: any) => {
      this.insuranceInfo = res;
      this.insuranceName = res.name;
    }, (error) => console.log(error));
  }
}
