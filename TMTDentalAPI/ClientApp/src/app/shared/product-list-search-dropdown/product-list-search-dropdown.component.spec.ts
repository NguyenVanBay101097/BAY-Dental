import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductListSearchDropdownComponent } from './product-list-search-dropdown.component';

describe('ProductListSearchDropdownComponent', () => {
  let component: ProductListSearchDropdownComponent;
  let fixture: ComponentFixture<ProductListSearchDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductListSearchDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductListSearchDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
