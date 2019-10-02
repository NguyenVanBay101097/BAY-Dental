import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductProductFormComponent } from './product-product-form.component';

describe('ProductProductFormComponent', () => {
  let component: ProductProductFormComponent;
  let fixture: ComponentFixture<ProductProductFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductProductFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductProductFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
