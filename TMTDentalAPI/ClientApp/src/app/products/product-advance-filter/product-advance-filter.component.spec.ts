import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductAdvanceFilterComponent } from './product-advance-filter.component';

describe('ProductAdvanceFilterComponent', () => {
  let component: ProductAdvanceFilterComponent;
  let fixture: ComponentFixture<ProductAdvanceFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductAdvanceFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductAdvanceFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
