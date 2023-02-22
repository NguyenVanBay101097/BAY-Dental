import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCategoryListComponent } from './partner-category-list.component';

describe('PartnerCategoryListComponent', () => {
  let component: PartnerCategoryListComponent;
  let fixture: ComponentFixture<PartnerCategoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCategoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCategoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
