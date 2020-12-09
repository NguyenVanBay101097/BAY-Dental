import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CardTypeService, CardTypeDisplay } from '../card-type.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PriceListService } from 'src/app/price-list/price-list.service';
import * as _ from 'lodash';

@Component({
  selector: 'app-card-type-create-update',
  templateUrl: './card-type-create-update.component.html',
  styleUrls: ['./card-type-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class CardTypeCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  cardType: CardTypeDisplay = new CardTypeDisplay();

  filteredPricelists: ProductPriceListBasic[];
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private cardTypeService: CardTypeService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private pricelistService: PriceListService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      basicPoint: 0,
      discount: [0, Validators.required],
      period: ['', Validators.required],
      nbPeriod: [0, Validators.required],
      pricelist: null
    });

    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadRecord();
    }

    // this.pricelistCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.pricelistCbx.loading = true)),
    //   switchMap(value => this.searchPricelists(value))
    // ).subscribe(result => {
    //   this.filteredPricelists = result.items;
    //   this.pricelistCbx.loading = false;
    // });

    // this.loadPricelists();
  }

  loadPricelists() {
    this.searchPricelists().subscribe(result => {
      this.filteredPricelists = _.unionBy(this.filteredPricelists, result.items, 'id');
    });
  }


  searchPricelists(filter?: string) {
    var val = new ProductPricelistPaged();
    val.search = filter || '';
    return this.pricelistService.loadPriceListList(val);
  }


  loadRecord() {
    this.cardTypeService.get(this.id).subscribe(result => {
      this.cardType = result;
      this.formGroup.patchValue(result);
    });
  }

  createNew() {
    this.router.navigate(['/card-types/create']);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    if (this.id) {
      this.cardTypeService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    } else {
      this.cardTypeService.create(value).subscribe(result => {
        this.router.navigate(['/card-types/edit/', result.id]);
      });
    }
  }

}
