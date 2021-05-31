import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationCreateUpdateFormComponent } from './quotation-create-update-form.component';

describe('QuotationCreateUpdateFormComponent', () => {
  let component: QuotationCreateUpdateFormComponent;
  let fixture: ComponentFixture<QuotationCreateUpdateFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QuotationCreateUpdateFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuotationCreateUpdateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
