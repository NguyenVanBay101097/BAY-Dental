import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductCategoryImportExcelDialogComponent } from './product-category-import-excel-dialog.component';

describe('ProductCategoryImportExcelDialogComponent', () => {
  let component: ProductCategoryImportExcelDialogComponent;
  let fixture: ComponentFixture<ProductCategoryImportExcelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductCategoryImportExcelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductCategoryImportExcelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
