import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductMedicineCuDialogComponent } from './product-medicine-cu-dialog.component';

describe('ProductMedicineCuDialogComponent', () => {
  let component: ProductMedicineCuDialogComponent;
  let fixture: ComponentFixture<ProductMedicineCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductMedicineCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductMedicineCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
