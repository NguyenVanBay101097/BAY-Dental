import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyCreateUpdateComponent } from './company-create-update.component';

describe('CompanyCreateUpdateComponent', () => {
  let component: CompanyCreateUpdateComponent;
  let fixture: ComponentFixture<CompanyCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompanyCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompanyCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
