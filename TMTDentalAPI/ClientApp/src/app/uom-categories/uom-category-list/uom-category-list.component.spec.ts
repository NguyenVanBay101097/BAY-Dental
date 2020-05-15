import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UomCategoryListComponent } from './uom-category-list.component';

describe('UomCategoryListComponent', () => {
  let component: UomCategoryListComponent;
  let fixture: ComponentFixture<UomCategoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UomCategoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UomCategoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
