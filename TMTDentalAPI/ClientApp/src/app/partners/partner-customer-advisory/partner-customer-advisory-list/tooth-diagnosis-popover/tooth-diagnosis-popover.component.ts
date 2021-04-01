import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDiagnosisPaged, ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-popover',
  templateUrl: './tooth-diagnosis-popover.component.html',
  styleUrls: ['./tooth-diagnosis-popover.component.css']
})
export class ToothDiagnosisPopoverComponent implements OnInit {

  allType = {
    diagnosis : 'diagnosis',
    service: 'service'
  }

  @Input() type = this.allType.diagnosis; // 'diagnosis': chẩn đoán ,'service': dịch vụ
  @Input() tags = [];
  // @Input() dataPopOver = []; //dataasource
  dataSource = [];
  @Input() title = '';
  // search_partnerCategory: string;
  searchUpdatePopOver = new Subject<string>();
  @Input() popOverPlace = 'right';

  // @Input() rowPartnerId: string;
  @Output() onSave = new EventEmitter();

  // @Output() otherOutput = new EventEmitter();
  // @Input() otherInput: any;

  @ViewChild('popOver', { static: true }) public popover: any;
  // @ViewChild("list", { static: true }) public list: MultiSelectComponent;


  constructor(
    private toothDiagnosisService: ToothDiagnosisService,
    private productService: ProductService
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.searchUpdatePopOver.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(value => {
          this.loadPartnerCategoryPopOver(value);
        });
    }, 200);
  }

  toggleWithTags(popover, mytags) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.loadPartnerCategoryPopOver();
      popover.open({ mytags });
    }
  }

  getPageDiagnosis(){
    var val = new ToothDiagnosisPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = '';
    this.toothDiagnosisService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  getPageProduct(){
    var val = new ProductPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  loadPartnerCategoryPopOver(q?: string) {
    if(this.type == this.allType.diagnosis) {
      this.getPageDiagnosis();
    }
    if(this.type == this.allType.service){
      this.getPageProduct();
    }
    // this.partnerCategoriesService.searchCombobox(q).subscribe((res: any) => {
    //   this.dataPopOver = res;
    // }, err => {
    //   console.log(err);
    // });
  }

  // // getValueDefault() {
  // //   const val = new PartnerCategoryPaged();
  // //   val.limit = 20;
  // //   val.offset = 0;
  // //   val.partnerId = this.rowPartnerId;
  // //   this.partnerCategoryService.getPaged(val).subscribe((res) => {
  // //     this.value_partnerCategoryPopOver = res.items;
  // //   });
  // // }

  update(tags) {
    tags = tags || [];
    this.popover.close();
    this.onSave.emit(tags);
  }

  // handleFilterCategoryPopOver(value) {
  //   this.search_partnerCategory = value;
  // }

  // public valueNormalizer = (text$: Observable<string>): any => text$.pipe(
  //   switchMap((text: string) => {
  //     // Search in values
  //     const matchingValue: any = this.tags.find((item: any) => {
  //       // Search for matching item to avoid duplicates
  //       return item['Name'].toLowerCase() === text.toLowerCase();
  //     });

  //     if (matchingValue) {
  //       // Return the already selected matching value and the component will remove it
  //       return of(matchingValue);
  //     }

  //     // Search in data
  //     const matchingItem: any = this.dataPopOver.find((item: any) => {
  //       return item['Name'].toLowerCase() === text.toLowerCase();
  //     });

  //     if (matchingItem) {
  //       return of(matchingItem);
  //     } else {
  //       return of(text).pipe(switchMap(this.service$));
  //     }
  //   })
  // )

  // public service$ = (text: string): any => {
  //   const val = new PartnerCategoryDisplay();
  //   val.name = text;
  //   return this.partnerCategoryService.create(val)
  //     .pipe(
  //       map((result: any) => {
  //         return {
  //           Id: result.id,
  //           Name: result.name
  //         }
  //       })
  //     );
  // }

}
