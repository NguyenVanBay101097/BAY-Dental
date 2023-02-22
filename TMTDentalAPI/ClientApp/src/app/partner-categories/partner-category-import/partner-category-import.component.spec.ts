import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCategoryImportComponent } from './partner-category-import.component';

describe('PartnerCategoryImportComponent', () => {
  let component: PartnerCategoryImportComponent;
  let fixture: ComponentFixture<PartnerCategoryImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCategoryImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCategoryImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
