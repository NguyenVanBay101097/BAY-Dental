import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductServiceCuDialogComponent } from './product-service-cu-dialog.component';

describe('ProductServiceCuDialogComponent', () => {
  let component: ProductServiceCuDialogComponent;
  let fixture: ComponentFixture<ProductServiceCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductServiceCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductServiceCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
