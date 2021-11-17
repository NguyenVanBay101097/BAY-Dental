import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { CardTypeService } from 'src/app/card-types/card-type.service';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductCategoriesSearchDropdownComponent } from 'src/app/shared/product-categories-search-dropdown/product-categories-search-dropdown.component';
import { ServiceCardTypeService } from '../../service-card-type.service';

@Component({
  selector: 'app-card-type-apply-cate-dialog',
  templateUrl: './card-type-apply-cate-dialog.component.html',
  styleUrls: ['./card-type-apply-cate-dialog.component.css']
})
export class CardTypeApplyCateDialogComponent implements OnInit {

  title: string;
  search: string = '';
  searchUpdate = new Subject<string>();
  form: FormGroup;
  searchProductCate = '';
  @ViewChild("searchCateComp", { static: true }) searchCateComp: ProductCategoriesSearchDropdownComponent;
  cateResult = [];
  cardTypeId: string;
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private productCategoryService: ProductCategoryService,
    private cardTypeService: ServiceCardTypeService,

    ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      productCategoryListItems: this.fb.array([])
    })
    
  }

  onApply(){
    var val = this.productCategoryListItems.value;
    this.cardTypeService.onApplyInCateg(this.cardTypeId,val).subscribe(() => {
      this.activeModal.close();
    })
  }

  changePeriod(){
  }

  addLine(event) {
    var list = this.productCategoryListItems.value;
    if (list.map(x => x.id).indexOf(event.id) != -1)
      return;

    var group = this.fb.group({
      categId: event.id,
      name: event.name,
      computePrice: 'percentage',
      percentPrice: 0,
      fixedAmountPrice: 0
    });

    this.productCategoryListItems.push(group);
    
  }

  deleteItem(itemId) {
    var listItem = this.form.get('productCategoryListItems') as FormArray;
    listItem.removeAt(itemId);
  }

  get productCategoryListItems() {
    return this.form.get('productCategoryListItems') as FormArray;
  }

  seachProductCategories$(search) {
    var val = new ProductCategoryPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = search ? search : '';
    val.type = 'service';
    if (val.search) {
      return this.productCategoryService.autocomplete(val).pipe(map((res: any) => {
        if (res.length > 0) {
          return res;
        } else {
          return [{ error: true }];
        }
      }));
    } else {
      return throwError({})
    }
  }

  changeComputePrice(event) {
  }

  onSearchCate(value)
  {
    this.searchProductCate = value;
    this.searchCateComp.searchResult$ = this.seachProductCategories$(this.searchProductCate);
  }

}
