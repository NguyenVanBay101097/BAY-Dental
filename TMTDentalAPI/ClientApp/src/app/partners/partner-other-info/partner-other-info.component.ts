import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { ServiceCardCardPaged } from 'src/app/service-card-cards/service-card-card-paged';
import { ServiceCardCardService } from 'src/app/service-card-cards/service-card-card.service';

@Component({
  selector: 'app-partner-other-info',
  templateUrl: './partner-other-info.component.html',
  styleUrls: ['./partner-other-info.component.css']
})
export class PartnerOtherInfoComponent implements OnInit {
  partnerId: string;
  preferentialCards = [];
  constructor(
     private activeRoute: ActivatedRoute,
     private cardCardService: ServiceCardCardService
  ) { }

  ngOnInit(): void {
    this.activeRoute.parent.params.subscribe((params) => {
      this.partnerId = params.id;
      this.loadPreferentialCards();
    });
  }

  loadPreferentialCards() {
    let val = new ServiceCardCardPaged();
    val.partnerId = this.partnerId;
    this.cardCardService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe((res) => {
      this.preferentialCards = res.data;
    });
  }
}
