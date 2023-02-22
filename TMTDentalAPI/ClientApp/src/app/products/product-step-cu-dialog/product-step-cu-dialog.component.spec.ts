import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductStepCuDialogComponent } from './product-step-cu-dialog.component';

describe('ProductStepCuDialogComponent', () => {
  let component: ProductStepCuDialogComponent;
  let fixture: ComponentFixture<ProductStepCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductStepCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductStepCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
