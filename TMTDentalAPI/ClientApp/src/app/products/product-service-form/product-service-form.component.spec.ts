import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductServiceFormComponent } from './product-service-form.component';

describe('ProductServiceFormComponent', () => {
  let component: ProductServiceFormComponent;
  let fixture: ComponentFixture<ProductServiceFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductServiceFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductServiceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
