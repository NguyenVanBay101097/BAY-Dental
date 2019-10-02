import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductImportExcelDialogComponent } from './product-import-excel-dialog.component';

describe('ProductImportExcelDialogComponent', () => {
  let component: ProductImportExcelDialogComponent;
  let fixture: ComponentFixture<ProductImportExcelDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductImportExcelDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductImportExcelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
