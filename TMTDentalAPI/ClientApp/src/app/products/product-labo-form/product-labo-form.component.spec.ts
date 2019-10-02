import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductLaboFormComponent } from './product-labo-form.component';

describe('ProductLaboFormComponent', () => {
  let component: ProductLaboFormComponent;
  let fixture: ComponentFixture<ProductLaboFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductLaboFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductLaboFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
