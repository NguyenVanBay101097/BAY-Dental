import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductStepFormComponent } from './product-step-form.component';

describe('ProductStepFormComponent', () => {
  let component: ProductStepFormComponent;
  let fixture: ComponentFixture<ProductStepFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductStepFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductStepFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
