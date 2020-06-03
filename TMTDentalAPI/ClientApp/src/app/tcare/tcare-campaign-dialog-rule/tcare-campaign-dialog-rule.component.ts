import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareRule } from '../tcare.service';
import { PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { map } from 'rxjs/operators';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-tcare-campaign-dialog-rule',
  templateUrl: './tcare-campaign-dialog-rule.component.html',
  styleUrls: ['./tcare-campaign-dialog-rule.component.css']
})
export class TcareCampaignDialogRuleComponent implements OnInit {

  title: string;
  formGroup: FormGroup;
  audience_filter: any;
  showAudienceFilter: boolean = false;

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      logic: "and",
      typeCondition: "",
      flagCondition: "",
      valueCondition: 0,
      nameCondition: "",
      selectedValueCondition: null,
    });
    this.showAudienceFilter = true;
  }

  saveAudienceFilter(event) {
    this.audience_filter = event;
  }
}
