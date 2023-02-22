import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductProductCuDialogComponent } from './product-product-cu-dialog.component';

describe('ProductProductCuDialogComponent', () => {
  let component: ProductProductCuDialogComponent;
  let fixture: ComponentFixture<ProductProductCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductProductCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductProductCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
