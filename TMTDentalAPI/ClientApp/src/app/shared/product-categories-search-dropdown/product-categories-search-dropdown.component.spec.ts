import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductCategoriesSearchDropdownComponent } from './product-categories-search-dropdown.component';

describe('ProductCategoriesSearchDropdownComponent', () => {
  let component: ProductCategoriesSearchDropdownComponent;
  let fixture: ComponentFixture<ProductCategoriesSearchDropdownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductCategoriesSearchDropdownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductCategoriesSearchDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
