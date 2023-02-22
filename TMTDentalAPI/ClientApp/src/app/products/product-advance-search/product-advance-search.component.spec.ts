import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductAdvanceSearchComponent } from './product-advance-search.component';

describe('ProductAdvanceSearchComponent', () => {
  let component: ProductAdvanceSearchComponent;
  let fixture: ComponentFixture<ProductAdvanceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductAdvanceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductAdvanceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
